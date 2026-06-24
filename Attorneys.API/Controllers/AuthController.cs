using System.Security.Claims;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Application.DTOs.Auth;
using Attorneys.Domain.Entities;
using Attorneys.Domain.Enums;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IJwtService _jwtService;

    public AuthController(ApplicationDbContext db, IJwtService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    [HttpPost("super-admin-login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> SuperAdminLogin(
        [FromBody] SuperAdminLoginRequest request,
        CancellationToken cancellationToken)
    {
        var userName = request.UserName.Trim().ToLowerInvariant();
        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u =>
                !u.IsDeleted &&
                u.UserName.ToLower() == userName &&
                u.Role == UserRole.SuperAdmin &&
                u.IsActive,
                cancellationToken);

        if (user is null)
            return Unauthorized("Invalid username or password.");

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var token = _jwtService.GenerateToken(user);
        return Ok(new LoginResponse(token, user.Role.ToString(), user.UserName, 0, null, null));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var firmCode = request.FirmCode.Trim().ToUpperInvariant();
        var userName = request.UserName.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(firmCode))
            return BadRequest("Firm code is required.");

        var tenant = await _db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IsActive && t.Code.ToUpper() == firmCode, cancellationToken);

        if (tenant is null)
            return Unauthorized("Invalid firm code.");

        var user = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u =>
                !u.IsDeleted &&
                u.TenantId == tenant.Id &&
                u.UserName.ToLower() == userName &&
                u.IsActive,
                cancellationToken);

        if (user is null)
            return Unauthorized("Invalid username or password.");

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid username or password.");

        var token = _jwtService.GenerateToken(user, tenant.Code, tenant.Name);
        return Ok(new LoginResponse(token, user.Role.ToString(), user.UserName, tenant.Id, tenant.Code, tenant.Name));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue("UserId") ?? "0");
        var user = await _db.UserSet.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return Unauthorized();

        var hasher = new PasswordHasher<User>();
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword) == PasswordVerificationResult.Failed)
            return BadRequest("Current password is incorrect.");

        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<object> Me()
    {
        return Ok(new
        {
            UserName = User.Identity?.Name,
            Role = User.FindFirstValue(ClaimTypes.Role),
            TenantId = User.FindFirstValue("TenantId"),
            FirmCode = User.FindFirstValue("FirmCode"),
            FirmName = User.FindFirstValue("FirmName")
        });
    }
}
