namespace Attorneys.Application.Common.Interfaces;

public interface IDocumentStorageService
{
    Task<(string StorageKey, long SizeBytes)> SaveAsync(int tenantId, string caseNo, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<(string StorageKey, long SizeBytes)> SaveWebsiteMediaAsync(int tenantId, string category, string fileName, Stream content, CancellationToken cancellationToken = default);
    Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
