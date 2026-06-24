using Attorneys.Application.DTOs.AI;

namespace Attorneys.Application.Common.Interfaces;

public interface IOpenAIService
{
    Task<DocumentAnalysisResult> AnalyzeDocumentAsync(string documentText, CancellationToken cancellationToken);
}
