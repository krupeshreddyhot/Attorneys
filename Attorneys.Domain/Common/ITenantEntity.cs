namespace Attorneys.Domain.Common;

public interface ITenantEntity
{
    int TenantId { get; set; }
    bool IsDeleted { get; set; }
}
