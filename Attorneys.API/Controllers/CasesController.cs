using Attorneys.Domain.Entities;
using Attorneys.Infrastructure.Persistence;
using Attorneys.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CasesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CasesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, CancellationToken cancellationToken)
    {
        var query = _db.Cases
            .Include(c => c.Court)
            .Include(c => c.CaseType)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.CaseNo.ToLower().Contains(term) ||
                (c.AppearingFor != null && c.AppearingFor.ToLower().Contains(term)) ||
                (c.OtherParty != null && c.OtherParty.ToLower().Contains(term)));
        }

        var cases = await query
            .OrderBy(c => c.CaseNo)
            .Select(c => new
            {
                c.CaseNo,
                c.CourtId,
                CourtName = c.Court != null ? c.Court.CourtName : null,
                c.CaseTypeId,
                CaseType = c.CaseType != null ? c.CaseType.Name : null,
                c.AppearingFor,
                c.ClientPhone,
                c.SerialNo,
                c.DateOfFiling,
                c.DateOfAppearance,
                DetailCount = c.Details.Count
            })
            .ToListAsync(cancellationToken);

        return Ok(cases);
    }

    [HttpGet("{caseNo}")]
    public async Task<IActionResult> GetByCaseNo(string caseNo, CancellationToken cancellationToken)
    {
        var legalCase = await _db.Cases
            .Include(c => c.Court)
            .Include(c => c.CaseType)
            .Include(c => c.Details)
            .FirstOrDefaultAsync(c => c.CaseNo == caseNo, cancellationToken);

        return legalCase is null ? NotFound() : Ok(legalCase);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CaseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CaseNo))
            return BadRequest("Case number is required.");

        var caseNo = request.CaseNo.Trim();
        if (await _db.Cases.AnyAsync(c => c.CaseNo == caseNo, cancellationToken))
            return Conflict("Case number already exists.");

        var legalCase = MapCase(new LegalCase { CaseNo = caseNo }, request);
        ApplyDetails(legalCase, request.Details);
        _db.CaseSet.Add(legalCase);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }

        return CreatedAtAction(nameof(GetByCaseNo), new { caseNo = legalCase.CaseNo }, legalCase);
    }

    [HttpPut("{caseNo}")]
    public async Task<IActionResult> Update(string caseNo, [FromBody] CaseRequest request, CancellationToken cancellationToken)
    {
        var legalCase = await _db.Cases
            .Include(c => c.Details)
            .FirstOrDefaultAsync(c => c.CaseNo == caseNo, cancellationToken);

        if (legalCase is null) return NotFound();

        MapCase(legalCase, request with { CaseNo = caseNo });

        if (request.Details is not null)
        {
            _db.CaseDetailSet.RemoveRange(legalCase.Details);
            ApplyDetails(legalCase, request.Details);
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }

        return Ok(legalCase);
    }

    [HttpDelete("{caseNo}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(string caseNo, CancellationToken cancellationToken)
    {
        var legalCase = await _db.CaseSet.FindAsync([caseNo], cancellationToken);
        if (legalCase is null) return NotFound();
        _db.CaseSet.Remove(legalCase);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodaysCases(CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var cases = await _db.CaseDetails
            .Include(d => d.Case)!.ThenInclude(c => c!.Court)
            .Include(d => d.Case)!.ThenInclude(c => c!.CaseType)
            .Where(d => d.NextDate.HasValue && d.NextDate.Value.Date == today)
            .OrderBy(d => d.Case!.Court!.CourtName)
            .Select(d => new
            {
                d.CaseNo,
                d.Stage,
                d.NextDate,
                d.Ia,
                CourtName = d.Case!.Court!.CourtName,
                CaseType = d.Case.CaseType != null ? d.Case.CaseType.Name : null,
                d.Case.AppearingFor
            })
            .ToListAsync(cancellationToken);

        return Ok(cases);
    }

    private static void ApplyDetails(LegalCase legalCase, List<CaseDetailRequest>? details)
    {
        if (details is null || details.Count == 0)
        {
            legalCase.Details = [];
            return;
        }

        legalCase.Details = details
            .Where(d => !string.IsNullOrWhiteSpace(d.Stage) || d.PreviousDate.HasValue || d.NextDate.HasValue)
            .Select((d, index) => new CaseDetail
            {
                TenantId = legalCase.TenantId,
                CaseNo = legalCase.CaseNo,
                CaseNoId = d.CaseNoId > 0 ? d.CaseNoId : index + 1,
                Stage = d.Stage,
                PreviousDate = DateTimeHelper.ToUtcDate(d.PreviousDate),
                NextDate = DateTimeHelper.ToUtcDate(d.NextDate),
                Ia = d.Ia,
                IaStage = d.IaStage
            })
            .ToList();
    }

    private static LegalCase MapCase(LegalCase legalCase, CaseRequest request)
    {
        legalCase.CourtId = request.CourtId;
        legalCase.CaseTypeId = request.CaseTypeId;
        legalCase.AppearingFor = request.AppearingFor;
        legalCase.ClientAddress = request.ClientAddress;
        legalCase.ClientPhone = request.ClientPhone;
        legalCase.SerialNo = request.SerialNo;
        legalCase.DateOfFiling = DateTimeHelper.ToUtcDate(request.DateOfFiling);
        legalCase.DateOfAppearance = DateTimeHelper.ToUtcDate(request.DateOfAppearance);
        legalCase.OtherParty = request.OtherParty;
        legalCase.CounselForOtherParty = request.CounselForOtherParty;
        legalCase.Remarks = request.Remarks;
        return legalCase;
    }

    public record CaseRequest(
        string CaseNo,
        string? CourtId,
        string? CaseTypeId,
        string? AppearingFor,
        string? ClientAddress,
        string? ClientPhone,
        string? SerialNo,
        DateTime? DateOfFiling,
        DateTime? DateOfAppearance,
        string? OtherParty,
        string? CounselForOtherParty,
        string? Remarks,
        List<CaseDetailRequest>? Details);

    public record CaseDetailRequest(
        int CaseNoId,
        string? Stage,
        DateTime? PreviousDate,
        DateTime? NextDate,
        string? Ia,
        string? IaStage);
}
