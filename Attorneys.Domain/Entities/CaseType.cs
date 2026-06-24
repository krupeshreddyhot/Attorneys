using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class CaseType : ITenantEntity

{

    public int TenantId { get; set; }

    public string CaseTypeId { get; set; } = string.Empty;

    public required string Name { get; set; }

    public bool IsDeleted { get; set; }



    public ICollection<LegalCase> Cases { get; set; } = new List<LegalCase>();

}

