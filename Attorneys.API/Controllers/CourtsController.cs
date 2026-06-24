using Attorneys.Application.Common.Interfaces;
using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CourtsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CourtsController(ApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var courts = await _db.Courts
            .OrderBy(c => c.CourtName)
            .Select(c => new { c.CourtId, c.CourtName, c.CourtCity })
            .ToListAsync(cancellationToken);
        return Ok(courts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var court = await FindCourtAsync(id, cancellationToken);
        return court is null ? NotFound() : Ok(court);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CourtRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.TenantId <= 0)
            return BadRequest("Tenant context is required.");

        if (await _db.Courts.AnyAsync(c => c.CourtId == request.CourtId, cancellationToken))
            return Conflict("Court ID already exists.");

        var court = new Domain.Entities.Court
        {
            TenantId = _currentUser.TenantId,
            CourtId = request.CourtId.Trim(),
            CourtName = request.CourtName.Trim(),
            CourtCity = request.CourtCity?.Trim()
        };
        _db.CourtSet.Add(court);
        await _db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = court.CourtId }, court);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(string id, [FromBody] CourtRequest request, CancellationToken cancellationToken)
    {
        var court = await FindCourtAsync(id, cancellationToken);
        if (court is null) return NotFound();

        court.CourtName = request.CourtName.Trim();
        court.CourtCity = request.CourtCity?.Trim();
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(court);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        var court = await FindCourtAsync(id, cancellationToken);
        if (court is null) return NotFound();
        _db.CourtSet.Remove(court);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<Domain.Entities.Court?> FindCourtAsync(string id, CancellationToken cancellationToken)
    {
        if (_currentUser.TenantId <= 0) return null;
        return await _db.CourtSet.FindAsync([_currentUser.TenantId, id], cancellationToken);
    }

    public record CourtRequest(string CourtId, string CourtName, string? CourtCity);
}
