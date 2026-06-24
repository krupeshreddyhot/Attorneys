using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class CaseDocument : ITenantEntity

{

    public int FileId { get; set; }

    public int TenantId { get; set; }

    public required string CaseNo { get; set; }

    public LegalCase? Case { get; set; }

    public required string FileName { get; set; }

    public string? Description { get; set; }

    public string? FileType { get; set; }

    public required string StorageKey { get; set; }

    public long FileSizeBytes { get; set; }

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; }

}

