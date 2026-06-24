using Attorneys.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CaseTypesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CaseTypesController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var types = await _db.CaseTypes
            .OrderBy(t => t.Name)
            .Select(t => new { t.CaseTypeId, t.Name })
            .ToListAsync(cancellationToken);
        return Ok(types);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CaseTypeRequest request, CancellationToken cancellationToken)
    {
        if (await _db.CaseTypes.AnyAsync(t => t.CaseTypeId == request.CaseTypeId, cancellationToken))
            return Conflict("Case type ID already exists.");

        var caseType = new Domain.Entities.CaseType
        {
            CaseTypeId = request.CaseTypeId.Trim(),
            Name = request.Name.Trim()
        };
        _db.CaseTypeSet.Add(caseType);
        await _db.SaveChangesAsync(cancellationToken);
        return Ok(caseType);
    }

    public record CaseTypeRequest(string CaseTypeId, string Name);
}
