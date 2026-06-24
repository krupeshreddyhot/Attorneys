using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class LegalCase : ITenantEntity

{

    public int TenantId { get; set; }

    public string CaseNo { get; set; } = string.Empty;

    public string? CourtId { get; set; }

    public Court? Court { get; set; }

    public string? CaseTypeId { get; set; }

    public CaseType? CaseType { get; set; }

    public string? AppearingFor { get; set; }

    public string? ClientAddress { get; set; }

    public string? ClientPhone { get; set; }

    public string? SerialNo { get; set; }

    public DateTime? DateOfFiling { get; set; }

    public DateTime? DateOfAppearance { get; set; }

    public string? OtherParty { get; set; }

    public string? CounselForOtherParty { get; set; }

    public string? Remarks { get; set; }

    public bool IsDeleted { get; set; }



    public ICollection<CaseDetail> Details { get; set; } = new List<CaseDetail>();

    public CaseAccount? Account { get; set; }

}

