using Attorneys.Application.Common.Interfaces;
using Attorneys.Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace Attorneys.Infrastructure.Services;

public class LocalDocumentStorageService : IDocumentStorageService
{
    private readonly string _rootPath;

    public LocalDocumentStorageService(IConfiguration configuration)
    {
        var configured = DocumentSettingsResolver.Get(configuration, "Documents:PhysicalRoot");
        _rootPath = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(AppContext.BaseDirectory, "App_Data", "documents")
            : configured;
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<(string StorageKey, long SizeBytes)> SaveAsync(
        int tenantId,
        string caseNo,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var safeCase = DocumentPathHelper.SanitizeCaseNo(caseNo);
        var folder = Path.Combine(_rootPath, tenantId.ToString(), safeCase);
        Directory.CreateDirectory(folder);

        var storageKey = $"{tenantId}/{safeCase}/{Guid.NewGuid():N}_{DocumentPathHelper.SanitizeFileName(fileName)}";
        var fullPath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, cancellationToken);
        return (storageKey, file.Length);
    }

    public async Task<(string StorageKey, long SizeBytes)> SaveWebsiteMediaAsync(
        int tenantId,
        string category,
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        var safeCategory = DocumentPathHelper.SanitizeCaseNo(category);
        var folder = Path.Combine(_rootPath, tenantId.ToString(), "website", safeCategory);
        Directory.CreateDirectory(folder);

        var storageKey = $"{tenantId}/website/{safeCategory}/{Guid.NewGuid():N}_{DocumentPathHelper.SanitizeFileName(fileName)}";
        var fullPath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));

        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, cancellationToken);
        return (storageKey, file.Length);
    }

    public Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath))
            return Task.FromResult<(Stream, string, string)?>(null);

        var fileName = Path.GetFileName(fullPath);
        var contentType = DocumentPathHelper.ContentTypeFromFileName(fileName);

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult<(Stream, string, string)?>((stream, contentType, fileName));
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
