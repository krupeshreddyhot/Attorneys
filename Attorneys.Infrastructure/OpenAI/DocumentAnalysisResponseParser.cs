using System.Text.Json;
using System.Text.RegularExpressions;
using Attorneys.Application.DTOs.AI;
using Attorneys.Infrastructure.OpenAI.Models;
using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.OpenAI;

public static partial class DocumentAnalysisResponseParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static DocumentAnalysisResult Parse(
        string responseContent,
        string model,
        string promptVersion,
        int inputTokens,
        int outputTokens,
        decimal estimatedCost,
        ILogger logger)
    {
        var parsed = TryDeserialize(responseContent, logger);
        if (parsed is null)
        {
            throw new InvalidOperationException("OpenAI returned malformed JSON for document analysis.");
        }

        return new DocumentAnalysisResult(
            Summary: parsed.Summary,
            KeyPoints: parsed.KeyPoints ?? [],
            Parties: parsed.Parties ?? [],
            ImportantDates: parsed.ImportantDates ?? [],
            NextActions: parsed.NextActions ?? [],
            AIModel: model,
            InputTokens: inputTokens,
            OutputTokens: outputTokens,
            EstimatedCost: estimatedCost,
            PromptVersion: promptVersion);
    }

    private static DocumentAnalysisChatResponse? TryDeserialize(string responseContent, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            logger.LogWarning("OpenAI document analysis response was empty.");
            return null;
        }

        foreach (var candidate in GetJsonCandidates(responseContent))
        {
            try
            {
                return JsonSerializer.Deserialize<DocumentAnalysisChatResponse>(candidate, JsonOptions);
            }
            catch (JsonException ex)
            {
                logger.LogWarning(ex, "Failed to parse OpenAI document analysis JSON candidate.");
            }
        }

        return null;
    }

    private static IEnumerable<string> GetJsonCandidates(string responseContent)
    {
        yield return responseContent.Trim();

        var fencedMatch = JsonCodeFenceRegex().Match(responseContent);
        if (fencedMatch.Success)
        {
            yield return fencedMatch.Groups[1].Value.Trim();
        }

        var objectMatch = JsonObjectRegex().Match(responseContent);
        if (objectMatch.Success)
        {
            yield return objectMatch.Value.Trim();
        }
    }

    [GeneratedRegex(@"```(?:json)?\s*([\s\S]*?)\s*```", RegexOptions.IgnoreCase)]
    private static partial Regex JsonCodeFenceRegex();

    [GeneratedRegex(@"\{[\s\S]*\}")]
    private static partial Regex JsonObjectRegex();
}
