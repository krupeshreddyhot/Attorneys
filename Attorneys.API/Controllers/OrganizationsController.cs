using Attorneys.Application.DTOs.Admin;
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
[Authorize(Roles = nameof(UserRole.SuperAdmin))]
public class OrganizationsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public OrganizationsController(ApplicationDbContext db) => _db = db;

    [HttpGet("firms")]
    public async Task<IActionResult> ListFirms(CancellationToken cancellationToken)
    {
        var firms = await _db.Tenants
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new { t.Id, t.Name, t.Code, t.IsActive, t.CreatedAtUtc })
            .ToListAsync(cancellationToken);

        var adminUsers = await _db.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => !u.IsDeleted && u.Role == UserRole.Administrator && u.TenantId > 0)
            .Select(u => new { u.TenantId, u.UserName, u.Id })
            .ToListAsync(cancellationToken);

        var adminByTenant = adminUsers
            .GroupBy(u => u.TenantId)
            .ToDictionary(g => g.Key, g => g.OrderBy(u => u.Id).First().UserName);

        var result = firms.Select(t => new FirmListItemDto(
            t.Id,
            t.Name,
            t.Code,
            t.IsActive,
            t.CreatedAtUtc,
            adminByTenant.GetValueOrDefault(t.Id))).ToList();

        return Ok(result);
    }

    [HttpPost("firms")]
    public async Task<ActionResult<ProvisionedTenantDto>> ProvisionFirm(
        [FromBody] ProvisionTenantRequest request,
        CancellationToken cancellationToken)
    {
        var firmName = request.FirmName.Trim();
        var firmCode = request.FirmCode.Trim().ToUpperInvariant();
        var adminUser = request.AdminUserName.Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(firmName) || string.IsNullOrWhiteSpace(firmCode))
            return BadRequest("Firm name and code are required.");
        if (string.IsNullOrWhiteSpace(adminUser) || string.IsNullOrWhiteSpace(request.AdminPassword))
            return BadRequest("Admin username and password are required.");

        if (await _db.Tenants.AnyAsync(t => t.Code.ToUpper() == firmCode, cancellationToken))
            return BadRequest("Firm code is already in use.");

        var tenant = new Tenant
        {
            Name = firmName,
            Code = firmCode,
            IsActive = true
        };
        _db.TenantSet.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);

        _db.CourtSet.AddRange(
            new Court { TenantId = tenant.Id, CourtId = "1", CourtName = "Sessions Court" },
            new Court { TenantId = tenant.Id, CourtId = "2", CourtName = "District Court" },
            new Court { TenantId = tenant.Id, CourtId = "3", CourtName = "High Court" });

        _db.CaseTypeSet.AddRange(
            new CaseType { TenantId = tenant.Id, CaseTypeId = "1", Name = "Criminal" },
            new CaseType { TenantId = tenant.Id, CaseTypeId = "2", Name = "Civil" },
            new CaseType { TenantId = tenant.Id, CaseTypeId = "3", Name = "Family" },
            new CaseType { TenantId = tenant.Id, CaseTypeId = "4", Name = "Bail" });

        var hasher = new PasswordHasher<User>();
        var admin = new User
        {
            TenantId = tenant.Id,
            UserName = adminUser,
            PasswordHash = string.Empty,
            Role = UserRole.Administrator,
            IsActive = true
        };
        admin.PasswordHash = hasher.HashPassword(admin, request.AdminPassword);
        _db.UserSet.Add(admin);

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new ProvisionedTenantDto(tenant.Id, tenant.Code, tenant.Name, admin.UserName));
    }

    [HttpPost("firms/{code}/reset-admin-password")]
    public async Task<IActionResult> ResetAdminPassword(
        string code,
        [FromBody] ResetAdminPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("New password is required.");

        var firmCode = code.Trim().ToUpperInvariant();
        var tenant = await _db.Tenants
            .FirstOrDefaultAsync(t => t.Code.ToUpper() == firmCode, cancellationToken);
        if (tenant is null)
            return NotFound("Firm not found.");

        var admin = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u =>
                !u.IsDeleted &&
                u.TenantId == tenant.Id &&
                u.Role == UserRole.Administrator,
                cancellationToken);

        if (admin is null)
            return NotFound("No administrator account exists for this firm.");

        var hasher = new PasswordHasher<User>();
        admin.PasswordHash = hasher.HashPassword(admin, request.NewPassword);
        admin.IsActive = true;
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { tenant.Code, admin.UserName });
    }
}
