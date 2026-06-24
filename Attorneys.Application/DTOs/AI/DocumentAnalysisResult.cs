namespace Attorneys.Application.DTOs.AI;

public record DocumentAnalysisResult(
    string? Summary,
    List<string> KeyPoints,
    List<string> Parties,
    List<string> ImportantDates,
    List<string> NextActions,
    string AIModel,
    int InputTokens,
    int OutputTokens,
    decimal EstimatedCost,
    string PromptVersion);
