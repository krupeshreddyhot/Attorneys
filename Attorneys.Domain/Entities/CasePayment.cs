using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class CasePayment : ITenantEntity

{

    public int Id { get; set; }

    public int TenantId { get; set; }

    public required string CaseNo { get; set; }

    public CaseAccount? Account { get; set; }

    public decimal AmountPaid { get; set; }

    public DateTime? PaidDate { get; set; }

    public bool IsDeleted { get; set; }

}

