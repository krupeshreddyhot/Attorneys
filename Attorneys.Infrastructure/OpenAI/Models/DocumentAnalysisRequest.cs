using Attorneys.Infrastructure.OpenAI;

namespace Attorneys.Infrastructure.OpenAI.Models;

public sealed class DocumentAnalysisRequest
{
    public required string DocumentText { get; init; }

    public string SystemPrompt { get; init; } = DocumentAnalysisPromptTemplate.SystemPrompt;

    public required string UserPrompt { get; init; }

    public static DocumentAnalysisRequest Create(string documentText) => new()
    {
        DocumentText = documentText,
        UserPrompt = DocumentAnalysisPromptTemplate.BuildUserPrompt(documentText)
    };
}
