using Attorneys.Application.DTOs.Website;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Domain.Entities;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/website")]
[Authorize(Roles = "Administrator")]
public class WebsiteController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IDocumentStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private static readonly HashSet<string> ImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public WebsiteController(
        ApplicationDbContext db,
        IDocumentStorageService storage,
        ICurrentUserService currentUser)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<WebsiteProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var tenant = await GetCurrentTenantAsync(cancellationToken);
        if (tenant is null) return NotFound();

        return Ok(MapProfile(tenant));
    }

    [HttpPut("profile")]
    public async Task<ActionResult<WebsiteProfileDto>> UpdateProfile(
        [FromBody] UpdateWebsiteProfileRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await GetCurrentTenantAsync(cancellationToken, tracked: true);
        if (tenant is null) return NotFound();

        tenant.AddressLine = request.AddressLine?.Trim();
        tenant.City = request.City?.Trim();
        tenant.Phone = request.Phone?.Trim();
        tenant.Email = request.Email?.Trim();
        tenant.HeroTagline = request.HeroTagline?.Trim();
        tenant.HeroSubtitle = request.HeroSubtitle?.Trim();
        tenant.AboutTitle = request.AboutTitle?.Trim();
        tenant.AboutBody = request.AboutBody?.Trim();
        tenant.AboutHighlightTitle = request.AboutHighlightTitle?.Trim();
        tenant.AboutHighlightBody = request.AboutHighlightBody?.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(MapProfile(tenant));
    }

    [HttpGet("banners")]
    public async Task<IActionResult> ListBanners(CancellationToken cancellationToken)
    {
        var tenantId = _currentUser.TenantId;
        var firmCode = await GetFirmCodeAsync(cancellationToken);

        var banners = await _db.TenantBannerImages.AsNoTracking()
            .OrderBy(b => b.SortOrder)
            .Select(b => new
            {
                b.Id,
                b.Caption,
                b.SortOrder,
                ImageUrl = $"/api/public/firms/{firmCode}/banners/{b.Id}/image",
            })
            .ToListAsync(cancellationToken);

        return Ok(banners);
    }

    [HttpPost("banners")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadBanner(
        [FromForm] string? caption,
        [FromForm] int sortOrder,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (_currentUser.TenantId <= 0)
            return BadRequest("Tenant context is required.");
        if (file.Length == 0)
            return BadRequest("File is empty.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!ImageExtensions.Contains(ext))
            return BadRequest("Allowed image types: JPG, JPEG, PNG, WEBP.");

        await using var stream = file.OpenReadStream();
        var (storageKey, _) = await _storage.SaveWebsiteMediaAsync(
            _currentUser.TenantId,
            "banners",
            file.FileName,
            stream,
            cancellationToken);

        var banner = new TenantBannerImage
        {
            StorageKey = storageKey,
            Caption = caption?.Trim(),
            SortOrder = sortOrder,
        };

        _db.TenantBannerImageSet.Add(banner);
        await _db.SaveChangesAsync(cancellationToken);

        var firmCode = await GetFirmCodeAsync(cancellationToken);
        return Ok(new
        {
            banner.Id,
            banner.Caption,
            banner.SortOrder,
            ImageUrl = firmCode != null ? $"/api/public/firms/{firmCode}/banners/{banner.Id}/image" : null,
        });
    }

    [HttpPut("banners/{id:int}")]
    public async Task<IActionResult> UpdateBanner(
        int id,
        [FromBody] UpdateBannerRequest request,
        CancellationToken cancellationToken)
    {
        var banner = await _db.TenantBannerImages.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (banner is null) return NotFound();

        banner.Caption = request.Caption?.Trim();
        banner.SortOrder = request.SortOrder;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("banners/{id:int}")]
    public async Task<IActionResult> DeleteBanner(int id, CancellationToken cancellationToken)
    {
        var banner = await _db.TenantBannerImages.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (banner is null) return NotFound();

        await _storage.DeleteAsync(banner.StorageKey, cancellationToken);
        banner.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("practice-areas")]
    public async Task<IActionResult> ListPracticeAreas(CancellationToken cancellationToken)
    {
        var areas = await _db.TenantPracticeAreas.AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .Select(p => new PracticeAreaDto(p.Id, p.Title, p.Description, p.SortOrder))
            .ToListAsync(cancellationToken);

        return Ok(areas);
    }

    [HttpPost("practice-areas")]
    public async Task<IActionResult> CreatePracticeArea(
        [FromBody] UpsertPracticeAreaRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var area = new TenantPracticeArea
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            SortOrder = request.SortOrder,
        };

        _db.TenantPracticeAreaSet.Add(area);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(new PracticeAreaDto(area.Id, area.Title, area.Description, area.SortOrder));
    }

    [HttpPut("practice-areas/{id:int}")]
    public async Task<IActionResult> UpdatePracticeArea(
        int id,
        [FromBody] UpsertPracticeAreaRequest request,
        CancellationToken cancellationToken)
    {
        var area = await _db.TenantPracticeAreas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (area is null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        area.Title = request.Title.Trim();
        area.Description = request.Description?.Trim();
        area.SortOrder = request.SortOrder;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("practice-areas/{id:int}")]
    public async Task<IActionResult> DeletePracticeArea(int id, CancellationToken cancellationToken)
    {
        var area = await _db.TenantPracticeAreas.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (area is null) return NotFound();

        area.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("advocates")]
    public async Task<IActionResult> ListAdvocates(CancellationToken cancellationToken)
    {
        var firmCode = await GetFirmCodeAsync(cancellationToken);
        var advocates = await _db.TenantAdvocates.AsNoTracking()
            .OrderBy(a => a.SortOrder)
            .Select(a => new AdvocateDto(
                a.Id,
                a.FullName,
                a.Designation,
                a.Bio,
                a.SortOrder,
                a.PhotoStorageKey != null && firmCode != null
                    ? $"/api/public/firms/{firmCode}/advocates/{a.Id}/photo"
                    : null))
            .ToListAsync(cancellationToken);

        return Ok(advocates);
    }

    [HttpPost("advocates")]
    public async Task<IActionResult> CreateAdvocate(
        [FromBody] UpsertAdvocateRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest("Full name is required.");

        var advocate = new TenantAdvocate
        {
            FullName = request.FullName.Trim(),
            Designation = request.Designation?.Trim(),
            Bio = request.Bio?.Trim(),
            SortOrder = request.SortOrder,
        };

        _db.TenantAdvocateSet.Add(advocate);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new AdvocateDto(advocate.Id, advocate.FullName, advocate.Designation, advocate.Bio, advocate.SortOrder, null));
    }

    [HttpPut("advocates/{id:int}")]
    public async Task<IActionResult> UpdateAdvocate(
        int id,
        [FromBody] UpsertAdvocateRequest request,
        CancellationToken cancellationToken)
    {
        var advocate = await _db.TenantAdvocates.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (advocate is null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest("Full name is required.");

        advocate.FullName = request.FullName.Trim();
        advocate.Designation = request.Designation?.Trim();
        advocate.Bio = request.Bio?.Trim();
        advocate.SortOrder = request.SortOrder;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("advocates/{id:int}/photo")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadAdvocatePhoto(
        int id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (_currentUser.TenantId <= 0)
            return BadRequest("Tenant context is required.");

        var advocate = await _db.TenantAdvocates.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (advocate is null) return NotFound();
        if (file.Length == 0) return BadRequest("File is empty.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!ImageExtensions.Contains(ext))
            return BadRequest("Allowed image types: JPG, JPEG, PNG, WEBP.");

        if (advocate.PhotoStorageKey is not null)
            await _storage.DeleteAsync(advocate.PhotoStorageKey, cancellationToken);

        await using var stream = file.OpenReadStream();
        var (storageKey, _) = await _storage.SaveWebsiteMediaAsync(
            _currentUser.TenantId,
            "advocates",
            file.FileName,
            stream,
            cancellationToken);

        advocate.PhotoStorageKey = storageKey;
        await _db.SaveChangesAsync(cancellationToken);

        var firmCode = await GetFirmCodeAsync(cancellationToken);
        return Ok(new { PhotoUrl = firmCode != null ? $"/api/public/firms/{firmCode}/advocates/{advocate.Id}/photo" : null });
    }

    [HttpDelete("advocates/{id:int}")]
    public async Task<IActionResult> DeleteAdvocate(int id, CancellationToken cancellationToken)
    {
        var advocate = await _db.TenantAdvocates.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (advocate is null) return NotFound();

        if (advocate.PhotoStorageKey is not null)
            await _storage.DeleteAsync(advocate.PhotoStorageKey, cancellationToken);

        advocate.IsDeleted = true;
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<Tenant?> GetCurrentTenantAsync(CancellationToken cancellationToken, bool tracked = false)
    {
        if (_currentUser.TenantId <= 0) return null;

        var query = tracked ? _db.Tenants : _db.Tenants.AsNoTracking();
        return await query.FirstOrDefaultAsync(t => t.Id == _currentUser.TenantId, cancellationToken);
    }

    private async Task<string?> GetFirmCodeAsync(CancellationToken cancellationToken)
    {
        var tenant = await GetCurrentTenantAsync(cancellationToken);
        return tenant?.Code;
    }

    private static WebsiteProfileDto MapProfile(Tenant tenant) =>
        new(
            tenant.Name,
            tenant.Code,
            tenant.AddressLine,
            tenant.City,
            tenant.Phone,
            tenant.Email,
            tenant.HeroTagline,
            tenant.HeroSubtitle,
            tenant.AboutTitle,
            tenant.AboutBody,
            tenant.AboutHighlightTitle,
            tenant.AboutHighlightBody);
}
