namespace Attorneys.Application.DTOs.Auth;



public record LoginRequest(string FirmCode, string UserName, string Password);



public record SuperAdminLoginRequest(string UserName, string Password);



public record LoginResponse(

    string Token,

    string Role,

    string UserName,

    int TenantId,

    string? FirmCode,

    string? FirmName);



public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

