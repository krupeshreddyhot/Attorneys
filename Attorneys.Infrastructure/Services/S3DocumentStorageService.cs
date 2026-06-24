using Amazon.S3;
using Amazon.S3.Model;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Infrastructure.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.Services;

public class S3DocumentStorageService : IDocumentStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<S3DocumentStorageService> _logger;
    private readonly string _keyPrefix;

    public S3DocumentStorageService(IConfiguration configuration, ILogger<S3DocumentStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _keyPrefix = (DocumentSettingsResolver.Get(configuration, "Documents:S3:KeyPrefix") ?? "cases").Trim().Trim('/');
    }

    public Task<(string StorageKey, long SizeBytes)> SaveAsync(
        int tenantId,
        string caseNo,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var safeCase = DocumentPathHelper.SanitizeCaseNo(caseNo);
        var storageKey = $"{tenantId}/{safeCase}/{Guid.NewGuid():N}_{DocumentPathHelper.SanitizeFileName(fileName)}";
        return SaveObjectAsync(storageKey, fileName, content, cancellationToken);
    }

    public Task<(string StorageKey, long SizeBytes)> SaveWebsiteMediaAsync(
        int tenantId,
        string category,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var safeCategory = DocumentPathHelper.SanitizeCaseNo(category);
        var storageKey = $"{tenantId}/website/{safeCategory}/{Guid.NewGuid():N}_{DocumentPathHelper.SanitizeFileName(fileName)}";
        return SaveObjectAsync(storageKey, fileName, content, cancellationToken);
    }

    private async Task<(string StorageKey, long SizeBytes)> SaveObjectAsync(
        string storageKey,
        string fileName,
        Stream content,
        CancellationToken cancellationToken)
    {
        var objectKey = BuildObjectKey(storageKey);
        var contentType = DocumentPathHelper.ContentTypeFromFileName(fileName);

        var (s3, bucket, useCustomEndpoint) = S3ClientFactory.Create(_configuration);
        using (s3)
        {
            await using var buffer = new MemoryStream();
            await content.CopyToAsync(buffer, cancellationToken);
            buffer.Position = 0;
            var size = buffer.Length;

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = objectKey,
                InputStream = buffer,
                ContentType = contentType,
            };

            if (useCustomEndpoint)
            {
                request.DisablePayloadSigning = true;
                request.DisableDefaultChecksumValidation = true;
            }

            try
            {
                var response = await s3.PutObjectAsync(request, cancellationToken);
                _ = response;
                return (storageKey, size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "S3 upload failed for key {ObjectKey} in bucket {Bucket}", objectKey, bucket);
                throw;
            }
        }
    }

    public async Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var objectKey = BuildObjectKey(storageKey);
        var (s3, bucket, _) = S3ClientFactory.Create(_configuration);
        using (s3)
        {
            try
            {
                using var response = await s3.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = objectKey,
                }, cancellationToken);

                var memory = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memory, cancellationToken);
                memory.Position = 0;

                var fileName = Path.GetFileName(storageKey);
                var contentType = !string.IsNullOrWhiteSpace(response.Headers.ContentType)
                    ? response.Headers.ContentType
                    : DocumentPathHelper.ContentTypeFromFileName(fileName);

                return (memory, contentType, fileName);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "S3 download failed for key {ObjectKey} in bucket {Bucket}", objectKey, bucket);
                throw;
            }
        }
    }

    public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var objectKey = BuildObjectKey(storageKey);
        var (s3, bucket, _) = S3ClientFactory.Create(_configuration);
        using (s3)
        {
            try
            {
                await s3.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = bucket,
                    Key = objectKey,
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "S3 delete failed for key {ObjectKey} in bucket {Bucket}", objectKey, bucket);
                throw;
            }
        }
    }

    private string BuildObjectKey(string storageKey) =>
        string.IsNullOrWhiteSpace(_keyPrefix) ? storageKey : $"{_keyPrefix}/{storageKey}";
}
