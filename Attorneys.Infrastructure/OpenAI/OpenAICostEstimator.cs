using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.OpenAI;

/// <summary>
/// Estimates OpenAI usage cost from configured per-million token prices.
/// OpenAI pricing changes over time and should be maintained via configuration.
/// </summary>
public static class OpenAICostEstimator
{
    public static decimal Estimate(
        int inputTokens,
        int outputTokens,
        decimal inputPerMillionTokens,
        decimal outputPerMillionTokens,
        ILogger logger)
    {
        // Formula:
        // (InputTokens / 1_000_000) * InputPrice + (OutputTokens / 1_000_000) * OutputPrice
        var inputCost = inputTokens / 1_000_000m * inputPerMillionTokens;
        var outputCost = outputTokens / 1_000_000m * outputPerMillionTokens;
        var estimatedCost = decimal.Round(inputCost + outputCost, 6);

        logger.LogInformation(
            "OpenAI cost estimate. InputTokens={InputTokens} OutputTokens={OutputTokens} EstimatedCost={EstimatedCost}",
            inputTokens,
            outputTokens,
            estimatedCost);

        return estimatedCost;
    }
}
