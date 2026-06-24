using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class CaseAccount : ITenantEntity

{

    public int TenantId { get; set; }

    public string CaseNo { get; set; } = string.Empty;

    public LegalCase? Case { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsDeleted { get; set; }



    public ICollection<CasePayment> Payments { get; set; } = new List<CasePayment>();

}

