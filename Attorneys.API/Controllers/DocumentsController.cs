using Attorneys.Application.Common.Interfaces;
using Attorneys.Domain.Entities;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IDocumentStorageService _storage;
    private readonly ICurrentUserService _currentUser;
    private static readonly HashSet<string> AllowedExtensions = [".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".txt"];

    public DocumentsController(
        ApplicationDbContext db,
        IDocumentStorageService storage,
        ICurrentUserService currentUser)
    {
        _db = db;
        _storage = storage;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? caseNo, CancellationToken cancellationToken)
    {
        var query = _db.CaseDocuments.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(caseNo))
            query = query.Where(d => d.CaseNo == caseNo);

        var docs = await query
            .OrderByDescending(d => d.UploadedAtUtc)
            .Select(d => new
            {
                d.FileId,
                d.CaseNo,
                d.FileName,
                d.Description,
                d.FileType,
                d.FileSizeBytes,
                d.UploadedAtUtc
            })
            .ToListAsync(cancellationToken);

        return Ok(docs);
    }

    [HttpPost("upload")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        [FromForm] string caseNo,
        [FromForm] string? description,
        [FromForm] string? fileType,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (_currentUser.TenantId <= 0)
            return BadRequest("Tenant context is required.");

        if (string.IsNullOrWhiteSpace(caseNo))
            return BadRequest("caseNo is required.");
        if (file.Length == 0)
            return BadRequest("File is empty.");

        var exists = await _db.Cases.AnyAsync(c => c.CaseNo == caseNo, cancellationToken);
        if (!exists)
            return NotFound("Case not found.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest("Allowed types: PDF, DOC, DOCX, JPG, PNG, TXT.");

        await using var stream = file.OpenReadStream();
        var (storageKey, size) = await _storage.SaveAsync(
            _currentUser.TenantId,
            caseNo,
            file.FileName,
            stream,
            cancellationToken);

        var doc = new CaseDocument
        {
            CaseNo = caseNo.Trim(),
            FileName = file.FileName,
            Description = description?.Trim(),
            FileType = fileType?.Trim() ?? ext.TrimStart('.'),
            StorageKey = storageKey,
            FileSizeBytes = size
        };

        _db.CaseDocumentSet.Add(doc);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { doc.FileId, doc.CaseNo, doc.FileName, doc.FileSizeBytes });
    }

    [HttpGet("{fileId:int}/download")]
    public async Task<IActionResult> Download(int fileId, CancellationToken cancellationToken)
    {
        var doc = await _db.CaseDocuments.AsNoTracking().FirstOrDefaultAsync(d => d.FileId == fileId, cancellationToken);
        if (doc is null) return NotFound();

        var opened = await _storage.OpenReadAsync(doc.StorageKey, cancellationToken);
        if (opened is null) return NotFound("File missing in storage.");

        return File(opened.Value.Stream, opened.Value.ContentType, doc.FileName);
    }

    [HttpDelete("{fileId:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int fileId, CancellationToken cancellationToken)
    {
        var doc = await _db.CaseDocuments.FirstOrDefaultAsync(d => d.FileId == fileId, cancellationToken);
        if (doc is null) return NotFound();

        await _storage.DeleteAsync(doc.StorageKey, cancellationToken);
        _db.CaseDocumentSet.Remove(doc);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
