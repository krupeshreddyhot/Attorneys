using System.Text.Json.Serialization;

using Attorneys.Domain.Common;



namespace Attorneys.Domain.Entities;



public class CaseDetail : ITenantEntity

{

    public int Id { get; set; }

    public int TenantId { get; set; }

    public required string CaseNo { get; set; }



    [JsonIgnore]

    public LegalCase? Case { get; set; }

    public int CaseNoId { get; set; }

    public string? Stage { get; set; }

    public DateTime? PreviousDate { get; set; }

    public DateTime? NextDate { get; set; }

    public string? Ia { get; set; }

    public string? IaStage { get; set; }

    public bool IsDeleted { get; set; }

}

