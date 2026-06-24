using System.Text.Json.Serialization;

namespace Attorneys.Infrastructure.OpenAI.Models;

public sealed class DocumentAnalysisChatResponse
{
    [JsonPropertyName("Summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("KeyPoints")]
    public List<string>? KeyPoints { get; set; }

    [JsonPropertyName("Parties")]
    public List<string>? Parties { get; set; }

    [JsonPropertyName("ImportantDates")]
    public List<string>? ImportantDates { get; set; }

    [JsonPropertyName("NextActions")]
    public List<string>? NextActions { get; set; }
}
