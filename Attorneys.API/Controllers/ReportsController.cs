using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ReportsController(ApplicationDbContext db) => _db = db;

    [HttpGet("today")]
    public Task<IActionResult> GetTodaysCases(CancellationToken cancellationToken) =>
        GetDiary(DateTime.Today, cancellationToken);

    [HttpGet("diary")]
    public async Task<IActionResult> GetDiary([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var targetDate = (date ?? DateTime.Today).Date;
        var rows = await _db.CaseDetails
            .AsNoTracking()
            .Include(d => d.Case)!.ThenInclude(c => c!.Court)
            .Include(d => d.Case)!.ThenInclude(c => c!.CaseType)
            .Where(d => d.NextDate.HasValue && d.NextDate.Value.Date == targetDate)
            .OrderBy(d => d.Case!.Court!.CourtName)
            .ThenBy(d => d.CaseNo)
            .Select(d => new ReportRow(
                d.CaseNo,
                d.Case!.Court!.CourtName,
                d.Case.CaseType != null ? d.Case.CaseType.Name : null,
                d.Case.AppearingFor,
                d.Stage,
                d.PreviousDate,
                d.NextDate,
                d.Ia,
                d.IaStage,
                d.Case.SerialNo))
            .ToListAsync(cancellationToken);

        return Ok(new { date = targetDate, rows });
    }

    [HttpGet("court-wise")]
    public async Task<IActionResult> GetCourtWise([FromQuery] string? courtId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(courtId))
            return BadRequest("courtId is required.");

        var rows = await _db.Cases
            .AsNoTracking()
            .Include(c => c.Court)
            .Include(c => c.CaseType)
            .Include(c => c.Details)
            .Where(c => c.CourtId == courtId)
            .OrderBy(c => c.CaseNo)
            .Select(c => new CourtWiseRow(
                c.CaseNo,
                c.Court!.CourtName,
                c.CaseType != null ? c.CaseType.Name : null,
                c.AppearingFor,
                c.SerialNo,
                c.DateOfFiling,
                c.Details.OrderByDescending(d => d.NextDate).Select(d => d.NextDate).FirstOrDefault(),
                c.Details.Count))
            .ToListAsync(cancellationToken);

        return Ok(rows);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var rows = await _db.CaseDetails
            .AsNoTracking()
            .Include(d => d.Case)!.ThenInclude(c => c!.Court)
            .Include(d => d.Case)!.ThenInclude(c => c!.CaseType)
            .Where(d => d.NextDate.HasValue && d.NextDate.Value.Date <= today)
            .OrderBy(d => d.NextDate)
            .Select(d => new ReportRow(
                d.CaseNo,
                d.Case!.Court!.CourtName,
                d.Case.CaseType != null ? d.Case.CaseType.Name : null,
                d.Case.AppearingFor,
                d.Stage,
                d.PreviousDate,
                d.NextDate,
                d.Ia,
                d.IaStage,
                d.Case.SerialNo))
            .ToListAsync(cancellationToken);

        return Ok(rows);
    }

    public record ReportRow(
        string CaseNo,
        string? CourtName,
        string? CaseType,
        string? AppearingFor,
        string? Stage,
        DateTime? PreviousDate,
        DateTime? NextDate,
        string? Ia,
        string? IaStage,
        string? SerialNo);

    public record CourtWiseRow(
        string CaseNo,
        string? CourtName,
        string? CaseType,
        string? AppearingFor,
        string? SerialNo,
        DateTime? DateOfFiling,
        DateTime? LatestNextDate,
        int HearingCount);
}
