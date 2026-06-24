using Attorneys.Domain.Common;

namespace Attorneys.Domain.Entities;

public class TenantBannerImage : ITenantEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string StorageKey { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
}
