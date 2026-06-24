using Attorneys.Domain.Common;

namespace Attorneys.Domain.Entities;

public class TenantPracticeArea : ITenantEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsDeleted { get; set; }
}
