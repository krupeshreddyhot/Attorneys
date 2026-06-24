using System.Security.Claims;
using Attorneys.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Attorneys.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        UserId = TryParseInt(user?.FindFirst("UserId")?.Value);
        Role = user?.FindFirst(ClaimTypes.Role)?.Value ?? "";
        TenantId = TryParseInt(user?.FindFirst("TenantId")?.Value);
    }

    public int UserId { get; }
    public string Role { get; }
    public int TenantId { get; }

    private static int TryParseInt(string? value) =>
        int.TryParse(value, out var result) ? result : 0;
}
