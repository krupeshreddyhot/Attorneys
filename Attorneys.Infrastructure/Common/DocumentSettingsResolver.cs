namespace Attorneys.Infrastructure.Common;

public static class DocumentSettingsResolver
{
    public static string? Get(Microsoft.Extensions.Configuration.IConfiguration configuration, string keyPath)
    {
        var value = configuration[keyPath]?.Trim();
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        var envDoubleUnderscore = keyPath.Replace(":", "__", StringComparison.Ordinal);
        value = Environment.GetEnvironmentVariable(envDoubleUnderscore)?.Trim();
        if (!string.IsNullOrWhiteSpace(value))
            return value;

        var envSingleUnderscore = keyPath.Replace(":", "_", StringComparison.Ordinal);
        value = Environment.GetEnvironmentVariable(envSingleUnderscore)?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
