using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class Court : ITenantEntity

{

    public int TenantId { get; set; }

    public string CourtId { get; set; } = string.Empty;

    public required string CourtName { get; set; }

    public string? CourtCity { get; set; }

    public bool IsDeleted { get; set; }



    public ICollection<LegalCase> Cases { get; set; } = new List<LegalCase>();

}

