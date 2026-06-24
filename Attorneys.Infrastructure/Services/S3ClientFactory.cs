using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Attorneys.Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace Attorneys.Infrastructure.Services;

internal static class S3ClientFactory
{
    public static (IAmazonS3 Client, string Bucket, bool UseCustomEndpoint) Create(IConfiguration configuration)
    {
        var bucket = DocumentSettingsResolver.Get(configuration, "Documents:S3:Bucket");
        if (string.IsNullOrWhiteSpace(bucket))
            throw new InvalidOperationException("Documents:S3:Bucket is required when Documents:Provider=s3.");

        var endpointRaw = DocumentSettingsResolver.Get(configuration, "Documents:S3:Endpoint");
        var regionName = DocumentSettingsResolver.Get(configuration, "Documents:S3:Region");
        var accessKey = DocumentSettingsResolver.Get(configuration, "Documents:S3:AccessKeyId");
        var secretKey = DocumentSettingsResolver.Get(configuration, "Documents:S3:SecretAccessKey");
        var forcePathStyleValue = DocumentSettingsResolver.Get(configuration, "Documents:S3:ForcePathStyle");
        var forcePathStyle = bool.TryParse(forcePathStyleValue, out var fps) && fps;

        var cfg = new AmazonS3Config
        {
            ForcePathStyle = forcePathStyle,
        };

        var useCustomEndpoint = !string.IsNullOrWhiteSpace(endpointRaw);
        if (useCustomEndpoint)
        {
            cfg.ServiceURL = NormalizeServiceUrl(StripOptionalBucketPath(endpointRaw!, bucket));
        }
        else if (!string.IsNullOrWhiteSpace(regionName)
                 && !string.Equals(regionName.Trim(), "auto", StringComparison.OrdinalIgnoreCase))
        {
            cfg.RegionEndpoint = RegionEndpoint.GetBySystemName(regionName.Trim());
        }

        IAmazonS3 client = string.IsNullOrWhiteSpace(accessKey) || string.IsNullOrWhiteSpace(secretKey)
            ? new AmazonS3Client(cfg)
            : new AmazonS3Client(new BasicAWSCredentials(accessKey, secretKey), cfg);

        return (client, bucket, useCustomEndpoint);
    }

    private static string NormalizeServiceUrl(string endpoint)
    {
        var trimmed = endpoint.Trim();
        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return trimmed;
        return $"https://{trimmed}";
    }

    private static string StripOptionalBucketPath(string endpoint, string bucket)
    {
        var trimmed = endpoint.Trim().TrimEnd('/');
        if (string.IsNullOrWhiteSpace(bucket))
            return trimmed;
        var suffix = "/" + bucket.Trim().Trim('/');
        if (trimmed.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            return trimmed[..^suffix.Length];
        return trimmed;
    }
}
