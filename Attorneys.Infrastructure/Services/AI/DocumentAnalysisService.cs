using System.Diagnostics;
using Attorneys.Application.Common.Interfaces;
using Attorneys.Application.DTOs.AI;
using Attorneys.Domain.Entities;
using Attorneys.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.Services.AI;

public class DocumentAnalysisService : IDocumentAnalysisService
{
    private const int MaxErrorMessageLength = 2000;

    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDocumentStorageService _storage;
    private readonly IDocumentTextExtractionService _textExtraction;
    private readonly IOpenAIService _openAi;
    private readonly ILogger<DocumentAnalysisService> _logger;

    public DocumentAnalysisService(
        IApplicationDbContext db,
        ICurrentUserService currentUser,
        IDocumentStorageService storage,
        IDocumentTextExtractionService textExtraction,
        IOpenAIService openAi,
        ILogger<DocumentAnalysisService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _storage = storage;
        _textExtraction = textExtraction;
        _openAi = openAi;
        _logger = logger;
    }

    public async Task QueueDocumentAnalysisAsync(int fileId, CancellationToken cancellationToken)
    {
        var tenantId = RequireTenantId();
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Queueing document analysis. FileId={FileId} TenantId={TenantId}",
            fileId,
            tenantId);

        var document = await _db.CaseDocuments
            .FirstOrDefaultAsync(d => d.FileId == fileId && d.TenantId == tenantId, cancellationToken);

        if (document is null)
        {
            throw new InvalidOperationException($"Case document {fileId} was not found.");
        }

        var analysis = await _db.CaseDocumentAIAnalyses
            .FirstOrDefaultAsync(a => a.FileId == fileId && a.TenantId == tenantId, cancellationToken);

        if (analysis?.Status == DocumentAnalysisStatus.Completed)
        {
            _logger.LogInformation(
                "Document analysis already completed. Skipping reprocessing. FileId={FileId} TenantId={TenantId} Status={Status}",
                fileId,
                tenantId,
                analysis.Status);

            return;
        }

        if (analysis?.Status == DocumentAnalysisStatus.Processing)
        {
            _logger.LogInformation(
                "Document analysis already in progress. FileId={FileId} TenantId={TenantId} Status={Status}",
                fileId,
                tenantId,
                analysis.Status);

            return;
        }

        if (analysis is null)
        {
            analysis = new CaseDocumentAIAnalysis
            {
                FileId = fileId,
                TenantId = tenantId,
                Status = DocumentAnalysisStatus.Processing,
                CreatedUtc = DateTime.UtcNow
            };

            _db.CaseDocumentAIAnalysisSet.Add(analysis);

            _logger.LogInformation(
                "Created document analysis record. FileId={FileId} TenantId={TenantId} Status={Status}",
                fileId,
                tenantId,
                analysis.Status);
        }
        else if (analysis.Status is DocumentAnalysisStatus.Failed or DocumentAnalysisStatus.Pending)
        {
            var previousStatus = analysis.Status;
            analysis.Status = DocumentAnalysisStatus.Processing;
            analysis.ErrorMessage = null;

            _logger.LogInformation(
                "Updated document analysis status. FileId={FileId} TenantId={TenantId} PreviousStatus={PreviousStatus} Status={Status}",
                fileId,
                tenantId,
                previousStatus,
                analysis.Status);
        }

        await _db.SaveChangesAsync(cancellationToken);

        try
        {
            var opened = await _storage.OpenReadAsync(document.StorageKey, cancellationToken);
            if (opened is null)
            {
                await MarkFailedAsync(
                    analysis,
                    "File missing in storage.",
                    fileId,
                    tenantId,
                    stopwatch,
                    cancellationToken);

                return;
            }

            await using var stream = opened.Value.Stream;
            var extractedText = await _textExtraction.ExtractTextAsync(
                stream,
                document.FileName,
                cancellationToken);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                await MarkFailedAsync(
                    analysis,
                    "No extractable text was found in the document.",
                    fileId,
                    tenantId,
                    stopwatch,
                    cancellationToken);

                return;
            }

            cancellationToken.ThrowIfCancellationRequested();

            var result = await _openAi.AnalyzeDocumentAsync(extractedText, cancellationToken);

            analysis.Summary = result.Summary;
            analysis.KeyPointsJson = DocumentAnalysisJsonSerializer.SerializeList(result.KeyPoints);
            analysis.PartiesJson = DocumentAnalysisJsonSerializer.SerializeList(result.Parties);
            analysis.ImportantDatesJson = DocumentAnalysisJsonSerializer.SerializeList(result.ImportantDates);
            analysis.NextActionsJson = DocumentAnalysisJsonSerializer.SerializeList(result.NextActions);
            analysis.AIModel = result.AIModel;
            analysis.PromptVersion = result.PromptVersion;
            analysis.InputTokens = result.InputTokens;
            analysis.OutputTokens = result.OutputTokens;
            analysis.EstimatedCost = result.EstimatedCost;
            analysis.ProcessedUtc = DateTime.UtcNow;
            analysis.Status = DocumentAnalysisStatus.Completed;
            analysis.ErrorMessage = null;

            await _db.SaveChangesAsync(cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation(
                "Document analysis completed. FileId={FileId} TenantId={TenantId} Status={Status} DurationMs={DurationMs}",
                fileId,
                tenantId,
                analysis.Status,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Document analysis failed. FileId={FileId} TenantId={TenantId} DurationMs={DurationMs}",
                fileId,
                tenantId,
                stopwatch.ElapsedMilliseconds);

            analysis.Status = DocumentAnalysisStatus.Failed;
            analysis.ErrorMessage = TruncateErrorMessage(ex.Message);

            await _db.SaveChangesAsync(cancellationToken);
            throw;
        }
    }

    public async Task<CaseDocumentAIAnalysisDto?> GetAnalysisAsync(int fileId, CancellationToken cancellationToken)
    {
        var tenantId = RequireTenantId();

        var analysis = await _db.CaseDocumentAIAnalyses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.FileId == fileId && a.TenantId == tenantId, cancellationToken);

        if (analysis is null)
        {
            return null;
        }

        return MapToDto(analysis);
    }

    public async Task RetryAnalysisAsync(int fileId, CancellationToken cancellationToken)
    {
        var tenantId = RequireTenantId();

        var analysis = await _db.CaseDocumentAIAnalyses
            .FirstOrDefaultAsync(a => a.FileId == fileId && a.TenantId == tenantId, cancellationToken);

        if (analysis is null)
        {
            throw new InvalidOperationException($"Document analysis for file {fileId} was not found.");
        }

        _logger.LogInformation(
            "Retrying document analysis. FileId={FileId} TenantId={TenantId} PreviousStatus={PreviousStatus}",
            fileId,
            tenantId,
            analysis.Status);

        analysis.Status = DocumentAnalysisStatus.Pending;
        analysis.ErrorMessage = null;
        analysis.ProcessedUtc = null;

        await _db.SaveChangesAsync(cancellationToken);

        await QueueDocumentAnalysisAsync(fileId, cancellationToken);
    }

    private int RequireTenantId()
    {
        if (_currentUser.TenantId <= 0)
        {
            throw new InvalidOperationException("Tenant context is required.");
        }

        return _currentUser.TenantId;
    }

    private async Task MarkFailedAsync(
        CaseDocumentAIAnalysis analysis,
        string errorMessage,
        int fileId,
        int tenantId,
        Stopwatch stopwatch,
        CancellationToken cancellationToken)
    {
        analysis.Status = DocumentAnalysisStatus.Failed;
        analysis.ErrorMessage = TruncateErrorMessage(errorMessage);

        await _db.SaveChangesAsync(cancellationToken);

        stopwatch.Stop();
        _logger.LogWarning(
            "Document analysis failed. FileId={FileId} TenantId={TenantId} Status={Status} DurationMs={DurationMs} ErrorMessage={ErrorMessage}",
            fileId,
            tenantId,
            analysis.Status,
            stopwatch.ElapsedMilliseconds,
            analysis.ErrorMessage);
    }

    private CaseDocumentAIAnalysisDto MapToDto(CaseDocumentAIAnalysis analysis) =>
        new(
            analysis.FileId,
            analysis.Status,
            analysis.Summary,
            DocumentAnalysisJsonSerializer.DeserializeList(analysis.KeyPointsJson, _logger),
            DocumentAnalysisJsonSerializer.DeserializeList(analysis.PartiesJson, _logger),
            DocumentAnalysisJsonSerializer.DeserializeList(analysis.ImportantDatesJson, _logger),
            DocumentAnalysisJsonSerializer.DeserializeList(analysis.NextActionsJson, _logger),
            analysis.AIModel,
            analysis.PromptVersion,
            analysis.CreatedUtc,
            analysis.ProcessedUtc);

    private static string TruncateErrorMessage(string message) =>
        message.Length <= MaxErrorMessageLength
            ? message
            : message[..MaxErrorMessageLength];
}
