using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db) => _db = db;

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken)
    {
        var today = DateTime.Today;
        var summary = new
        {
            TotalCases = await _db.Cases.CountAsync(cancellationToken),
            TotalCourts = await _db.Courts.CountAsync(cancellationToken),
            TodaysHearings = await _db.CaseDetails.CountAsync(d => d.NextDate.HasValue && d.NextDate.Value.Date == today, cancellationToken),
            TotalAccounts = await _db.CaseAccounts.CountAsync(cancellationToken)
        };
        return Ok(summary);
    }
}
