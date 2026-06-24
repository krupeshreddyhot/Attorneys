using Attorneys.Domain.Common;

namespace Attorneys.Domain.Entities;

public class TenantAdvocate : ITenantEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string FullName { get; set; }
    public string? Designation { get; set; }
    public string? PhotoStorageKey { get; set; }
    public string? Bio { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
}
