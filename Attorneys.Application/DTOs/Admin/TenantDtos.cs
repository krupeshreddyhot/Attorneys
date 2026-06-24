namespace Attorneys.Application.DTOs.Admin;



public record ProvisionTenantRequest(

    string FirmName,

    string FirmCode,

    string AdminUserName,

    string AdminPassword);



public record ProvisionedTenantDto(
    int TenantId,
    string FirmCode,
    string FirmName,
    string AdminUserName);

public record ResetAdminPasswordRequest(string NewPassword);

public record FirmListItemDto(
    int Id,
    string Name,
    string Code,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? AdminUserName);

