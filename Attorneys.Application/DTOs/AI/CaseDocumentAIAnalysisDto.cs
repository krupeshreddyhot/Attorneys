using Attorneys.Domain.Enums;

namespace Attorneys.Application.DTOs.AI;

public record CaseDocumentAIAnalysisDto(
    int FileId,
    DocumentAnalysisStatus Status,
    string? Summary,
    List<string> KeyPoints,
    List<string> Parties,
    List<string> ImportantDates,
    List<string> NextActions,
    string? AIModel,
    string? PromptVersion,
    DateTime CreatedUtc,
    DateTime? ProcessedUtc);
