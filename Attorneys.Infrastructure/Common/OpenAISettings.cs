using Microsoft.Extensions.Configuration;

namespace Attorneys.Infrastructure.Common;

public static class OpenAISettings
{
    public const string DefaultModel = "gpt-5.4-mini";
    public const int DefaultMaxCharacters = 100_000;
    public const int MinMaxCharacters = 1_000;
    public const int MaxMaxCharacters = 500_000;
    public const float DefaultTemperature = 0.1f;

    public static string? GetApiKey(IConfiguration configuration) =>
        DocumentSettingsResolver.Get(configuration, "OpenAI:ApiKey");

    public static string GetModel(IConfiguration configuration) =>
        DocumentSettingsResolver.Get(configuration, "OpenAI:Model") ?? DefaultModel;

    public static int GetMaxCharacters(IConfiguration configuration) =>
        ResolveMaxCharacters(DocumentSettingsResolver.Get(configuration, "OpenAI:MaxCharacters"));

    public static int ResolveMaxCharacters(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out var parsed))
        {
            return DefaultMaxCharacters;
        }

        if (parsed < MinMaxCharacters || parsed > MaxMaxCharacters)
        {
            return DefaultMaxCharacters;
        }

        return parsed;
    }

    /// <summary>
    /// Returns configured input price per 1M tokens.
    /// OpenAI pricing changes over time and should be maintained via configuration.
    /// </summary>
    public static decimal GetInputPerMillionTokens(IConfiguration configuration) =>
        GetRequiredPricingValue(configuration, "OpenAI:Pricing:InputPerMillionTokens");

    /// <summary>
    /// Returns configured output price per 1M tokens.
    /// OpenAI pricing changes over time and should be maintained via configuration.
    /// </summary>
    public static decimal GetOutputPerMillionTokens(IConfiguration configuration) =>
        GetRequiredPricingValue(configuration, "OpenAI:Pricing:OutputPerMillionTokens");

    public static float GetTemperature(IConfiguration configuration) =>
        ResolveTemperature(DocumentSettingsResolver.Get(configuration, "OpenAI:Temperature"));

    public static float ResolveTemperature(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !float.TryParse(value, out var parsed))
        {
            return DefaultTemperature;
        }

        if (parsed < 0f || parsed > 2f)
        {
            return DefaultTemperature;
        }

        return parsed;
    }

    private static decimal GetRequiredPricingValue(IConfiguration configuration, string keyPath)
    {
        var value = DocumentSettingsResolver.Get(configuration, keyPath);
        if (string.IsNullOrWhiteSpace(value) || !decimal.TryParse(value, out var parsed) || parsed < 0)
        {
            throw new InvalidOperationException(
                $"{keyPath} is not configured. OpenAI pricing changes over time and should be maintained via configuration.");
        }

        return parsed;
    }
}
