using Attorneys.Application.DTOs.AI;

namespace Attorneys.Application.Common.Interfaces;

public interface IDocumentAnalysisService
{
    Task QueueDocumentAnalysisAsync(int fileId, CancellationToken cancellationToken);

    Task<CaseDocumentAIAnalysisDto?> GetAnalysisAsync(int fileId, CancellationToken cancellationToken);

    Task RetryAnalysisAsync(int fileId, CancellationToken cancellationToken);
}
