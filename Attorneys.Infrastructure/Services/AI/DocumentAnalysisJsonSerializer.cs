using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.Services.AI;

internal static class DocumentAnalysisJsonSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new();

    public static string SerializeList(IReadOnlyList<string> items) =>
        JsonSerializer.Serialize(items, JsonOptions);

    public static List<string> DeserializeList(string? json, ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? [];
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to deserialize document analysis JSON list.");
            return [];
        }
    }
}
