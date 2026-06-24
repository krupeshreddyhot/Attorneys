using Attorneys.Domain.Entities;
using Attorneys.Infrastructure.Persistence;
using Attorneys.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attorneys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AccountsController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public AccountsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var accounts = await _db.CaseAccounts
            .Include(a => a.Payments)
            .OrderBy(a => a.CaseNo)
            .Select(a => new
            {
                a.CaseNo,
                a.TotalAmount,
                PaidTotal = a.Payments.Sum(p => p.AmountPaid),
                Balance = a.TotalAmount - a.Payments.Sum(p => p.AmountPaid),
                Payments = a.Payments.OrderBy(p => p.PaidDate).Select(p => new { p.Id, p.AmountPaid, p.PaidDate })
            })
            .ToListAsync(cancellationToken);

        return Ok(accounts);
    }

    [HttpGet("{caseNo}")]
    public async Task<IActionResult> GetByCaseNo(string caseNo, CancellationToken cancellationToken)
    {
        var account = await _db.CaseAccounts
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.CaseNo == caseNo, cancellationToken);

        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] AccountRequest request, CancellationToken cancellationToken)
    {
        var account = await _db.CaseAccounts
            .Include(a => a.Payments)
            .FirstOrDefaultAsync(a => a.CaseNo == request.CaseNo, cancellationToken);

        if (account is null)
        {
            account = new CaseAccount { CaseNo = request.CaseNo.Trim(), TotalAmount = request.TotalAmount };
            _db.CaseAccountSet.Add(account);
        }
        else
        {
            account.TotalAmount = request.TotalAmount;
        }

        if (request.Payments is not null)
        {
            _db.CasePaymentSet.RemoveRange(account.Payments);
            account.Payments = request.Payments.Select(p => new CasePayment
            {
                CaseNo = request.CaseNo,
                AmountPaid = p.AmountPaid,
                PaidDate = DateTimeHelper.ToUtcDate(p.PaidDate)
            }).ToList();
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(account);
    }

    public record AccountRequest(string CaseNo, decimal TotalAmount, List<PaymentRequest>? Payments);
    public record PaymentRequest(decimal AmountPaid, DateTime? PaidDate);
}
