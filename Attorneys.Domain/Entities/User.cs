using Attorneys.Domain.Common;

using Attorneys.Domain.Enums;



namespace Attorneys.Domain.Entities;



public class User : ITenantEntity

{

    public int Id { get; set; }

    public int TenantId { get; set; }

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

}

