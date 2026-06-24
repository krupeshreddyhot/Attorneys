using Attorneys.Application.Common.Interfaces;
using Attorneys.Application.DTOs.AI;
using Attorneys.Application.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attorneys.API.Controllers;

/// <summary>
/// AI document analysis endpoints for case documents.
/// </summary>
[ApiController]
[Route("api/documents")]
[Authorize]
public class DocumentAnalysisController : ControllerBase
{
    private readonly IDocumentAnalysisService _documentAnalysisService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DocumentAnalysisController> _logger;

    public DocumentAnalysisController(
        IDocumentAnalysisService documentAnalysisService,
        ICurrentUserService currentUser,
        ILogger<DocumentAnalysisController> logger)
    {
        _documentAnalysisService = documentAnalysisService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Analyze — queue AI analysis for a document.
    /// </summary>
    /// <remarks>
    /// Starts document text extraction and OpenAI analysis for the specified file.
    /// Returns immediately while processing continues in the request pipeline.
    /// </remarks>
    [HttpPost("{fileId:int}/analyze")]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Analyze(int fileId, CancellationToken cancellationToken)
    {
        LogEndpoint(nameof(Analyze), fileId);

        await _documentAnalysisService.QueueDocumentAnalysisAsync(fileId, cancellationToken);

        return Accepted(new MessageResponseDto("Document analysis queued."));
    }

    /// <summary>
    /// Get Analysis — retrieve AI analysis results for a document.
    /// </summary>
    /// <remarks>
    /// Returns the latest analysis summary, key points, parties, dates, and next actions when available.
    /// </remarks>
    [HttpGet("{fileId:int}/analysis")]
    [ProducesResponseType(typeof(CaseDocumentAIAnalysisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnalysis(int fileId, CancellationToken cancellationToken)
    {
        LogEndpoint(nameof(GetAnalysis), fileId);

        var analysis = await _documentAnalysisService.GetAnalysisAsync(fileId, cancellationToken);
        if (analysis is null)
        {
            return NotFound(new MessageResponseDto("Analysis not found."));
        }

        return Ok(analysis);
    }

    /// <summary>
    /// Retry Analysis — re-queue analysis after a failure or for a fresh run.
    /// </summary>
    /// <remarks>
    /// Resets a failed or pending analysis and queues processing again.
    /// </remarks>
    [HttpPost("{fileId:int}/retry-analysis")]
    [ProducesResponseType(typeof(MessageResponseDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RetryAnalysis(int fileId, CancellationToken cancellationToken)
    {
        LogEndpoint(nameof(RetryAnalysis), fileId);

        await _documentAnalysisService.RetryAnalysisAsync(fileId, cancellationToken);

        return Accepted(new MessageResponseDto("Document analysis retry queued."));
    }

    private void LogEndpoint(string endpoint, int fileId)
    {
        _logger.LogInformation(
            "Document analysis endpoint invoked. Endpoint={Endpoint} FileId={FileId} UserId={UserId}",
            endpoint,
            fileId,
            _currentUser.UserId);
    }
}
