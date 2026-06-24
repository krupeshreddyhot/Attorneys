using Attorneys.Domain.Common;
using Attorneys.Domain.Enums;

namespace Attorneys.Domain.Entities;

public class CaseDocumentAIAnalysis : ITenantEntity
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int FileId { get; set; }

    public CaseDocument? Document { get; set; }

    public DocumentAnalysisStatus Status { get; set; } = DocumentAnalysisStatus.Pending;

    public string? Summary { get; set; }

    public string? KeyPointsJson { get; set; }

    public string? PartiesJson { get; set; }

    public string? ImportantDatesJson { get; set; }

    public string? NextActionsJson { get; set; }

    public string? AIModel { get; set; }

    public string? PromptVersion { get; set; }

    public int? InputTokens { get; set; }

    public int? OutputTokens { get; set; }

    public decimal? EstimatedCost { get; set; }

    public string? ExtractedTextStorageKey { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedUtc { get; set; }

    public string? ErrorMessage { get; set; }

    public bool IsDeleted { get; set; }
}
