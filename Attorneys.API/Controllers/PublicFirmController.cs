using Attorneys.Application.DTOs.Website;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Domain.Entities;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/public/firms")]
[AllowAnonymous]
public class PublicFirmController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public PublicFirmController(ApplicationDbContext db) => _db = db;

    [HttpGet("{code}/landing")]
    public async Task<ActionResult<FirmLandingDto>> GetLanding(string code, CancellationToken cancellationToken)
    {
        var firmCode = code.Trim().ToUpperInvariant();
        var tenant = await _db.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == firmCode && t.IsActive, cancellationToken);

        if (tenant is null)
            return NotFound();

        var banners = await _db.TenantBannerImages.IgnoreQueryFilters().AsNoTracking()
            .Where(b => b.TenantId == tenant.Id && !b.IsDeleted)
            .OrderBy(b => b.SortOrder)
            .Select(b => new BannerDto(
                b.Id,
                b.Caption,
                b.SortOrder,
                BuildBannerUrl(firmCode, b.Id)))
            .ToListAsync(cancellationToken);

        var practiceAreas = await _db.TenantPracticeAreas.IgnoreQueryFilters().AsNoTracking()
            .Where(p => p.TenantId == tenant.Id && !p.IsDeleted)
            .OrderBy(p => p.SortOrder)
            .Select(p => new PracticeAreaDto(p.Id, p.Title, p.Description, p.SortOrder))
            .ToListAsync(cancellationToken);

        var advocates = await _db.TenantAdvocates.IgnoreQueryFilters().AsNoTracking()
            .Where(a => a.TenantId == tenant.Id && !a.IsDeleted)
            .OrderBy(a => a.SortOrder)
            .Select(a => new AdvocateDto(
                a.Id,
                a.FullName,
                a.Designation,
                a.Bio,
                a.SortOrder,
                a.PhotoStorageKey != null ? BuildAdvocatePhotoUrl(firmCode, a.Id) : null))
            .ToListAsync(cancellationToken);

        return Ok(new FirmLandingDto(
            tenant.Code,
            tenant.Name,
            tenant.AddressLine,
            tenant.City,
            tenant.Phone,
            tenant.Email,
            tenant.HeroTagline,
            tenant.HeroSubtitle,
            tenant.AboutTitle,
            tenant.AboutBody,
            tenant.AboutHighlightTitle,
            tenant.AboutHighlightBody,
            banners,
            practiceAreas,
            advocates));
    }

    [HttpGet("{code}/banners/{id:int}/image")]
    public async Task<IActionResult> GetBannerImage(string code, int id, CancellationToken cancellationToken)
    {
        var tenant = await ResolveTenantAsync(code, cancellationToken);
        if (tenant is null) return NotFound();

        var banner = await _db.TenantBannerImages.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id && b.TenantId == tenant.Id && !b.IsDeleted, cancellationToken);
        if (banner is null) return NotFound();

        return await ServeImageAsync(banner.StorageKey, cancellationToken);
    }

    [HttpGet("{code}/advocates/{id:int}/photo")]
    public async Task<IActionResult> GetAdvocatePhoto(string code, int id, CancellationToken cancellationToken)
    {
        var tenant = await ResolveTenantAsync(code, cancellationToken);
        if (tenant is null) return NotFound();

        var advocate = await _db.TenantAdvocates.IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenant.Id && !a.IsDeleted, cancellationToken);
        if (advocate is null || advocate.PhotoStorageKey is null) return NotFound();

        return await ServeImageAsync(advocate.PhotoStorageKey, cancellationToken);
    }

    private async Task<Tenant?> ResolveTenantAsync(string code, CancellationToken cancellationToken)
    {
        var firmCode = code.Trim().ToUpperInvariant();
        return await _db.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == firmCode && t.IsActive, cancellationToken);
    }

    private async Task<IActionResult> ServeImageAsync(string storageKey, CancellationToken cancellationToken)
    {
        var storage = HttpContext.RequestServices.GetRequiredService<IDocumentStorageService>();
        var opened = await storage.OpenReadAsync(storageKey, cancellationToken);
        if (opened is null) return NotFound();

        return File(opened.Value.Stream, opened.Value.ContentType);
    }

    private static string BuildBannerUrl(string firmCode, int id) =>
        $"/api/public/firms/{firmCode}/banners/{id}/image";

    private static string BuildAdvocatePhotoUrl(string firmCode, int id) =>
        $"/api/public/firms/{firmCode}/advocates/{id}/photo";
}
