using System.Text.Json;
using Attorneys.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var contentRoot = AppContext.BaseDirectory;
var pdfPath = ResolvePdfPath(args);
if (!File.Exists(pdfPath))
{
    Console.WriteLine($"PDF not found: {pdfPath}");
    Console.WriteLine("Usage: dotnet run --project Attorneys.AI.ManualTest -- <path-to.pdf>");
    return 1;
}

var config = new ConfigurationBuilder()
    .SetBasePath(contentRoot)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile("appsettings.Development.json", optional: true)
    .AddJsonFile(ResolveOptionalConfigPath("Attorneys.API", "appsettings.json"), optional: true)
    .AddJsonFile(ResolveOptionalConfigPath("Attorneys.API", "appsettings.Development.json"), optional: true)
    .AddEnvironmentVariables()
    .Build();
using var loggerFactory = LoggerFactory.Create(builder =>
    builder.AddConsole().SetMinimumLevel(LogLevel.Information));

var extractor = new DocumentTextExtractionService();

Console.WriteLine($"PDF: {pdfPath}");
Console.WriteLine($"Model: {config["OpenAI:Model"] ?? "(default)"}");
Console.WriteLine();

Console.WriteLine("Step 1/2: Extracting text...");
await using var stream = File.OpenRead(pdfPath);
var text = await extractor.ExtractTextAsync(stream, Path.GetFileName(pdfPath), CancellationToken.None);

Console.WriteLine($"Extracted characters: {text.Length}");
if (text.Length == 0)
{
    Console.WriteLine("No text extracted. OpenAI step skipped.");
    return 2;
}

var previewLength = Math.Min(400, text.Length);
Console.WriteLine($"Preview: {text[..previewLength]}");
Console.WriteLine();

if (string.IsNullOrWhiteSpace(config["OpenAI:ApiKey"]))
{
    Console.WriteLine("OpenAI:ApiKey is not configured. Set OpenAI__ApiKey or appsettings.Development.json.");
    Console.WriteLine("Text extraction succeeded. Skipping OpenAI step.");
    return 3;
}

Console.WriteLine("Step 2/2: Calling OpenAI...");
var openAi = new OpenAIService(config, loggerFactory.CreateLogger<OpenAIService>());
var result = await openAi.AnalyzeDocumentAsync(text, CancellationToken.None);

Console.WriteLine();
Console.WriteLine("=== DocumentAnalysisResult ===");
Console.WriteLine($"Summary: {result.Summary}");
Console.WriteLine($"AIModel: {result.AIModel}");
Console.WriteLine($"PromptVersion: {result.PromptVersion}");
Console.WriteLine($"InputTokens: {result.InputTokens}");
Console.WriteLine($"OutputTokens: {result.OutputTokens}");
Console.WriteLine($"EstimatedCost: {result.EstimatedCost}");
Console.WriteLine($"KeyPoints ({result.KeyPoints.Count}):");
foreach (var item in result.KeyPoints.Take(5))
{
    Console.WriteLine($"  - {item}");
}

Console.WriteLine($"Parties ({result.Parties.Count}):");
foreach (var item in result.Parties.Take(5))
{
    Console.WriteLine($"  - {item}");
}

Console.WriteLine($"ImportantDates ({result.ImportantDates.Count}):");
foreach (var item in result.ImportantDates.Take(5))
{
    Console.WriteLine($"  - {item}");
}

Console.WriteLine($"NextActions ({result.NextActions.Count}):");
foreach (var item in result.NextActions.Take(5))
{
    Console.WriteLine($"  - {item}");
}

Console.WriteLine();
Console.WriteLine("Raw JSON:");
Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));

return 0;

static string ResolveOptionalConfigPath(string projectFolder, string fileName)
{
    var candidates = new[]
    {
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", projectFolder, fileName)),
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectFolder, fileName)),
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", projectFolder, fileName))
    };

    return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
}

static string ResolvePdfPath(string[] args)
{
    if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
    {
        return Path.GetFullPath(args[0]);
    }

    var candidates = new[]
    {
        Path.Combine(AppContext.BaseDirectory, "TestAssets", "sample-legal.pdf"),
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "TestAssets", "sample-legal.pdf"),
        Path.Combine(Directory.GetCurrentDirectory(), "Attorneys.AI.ManualTest", "TestAssets", "sample-legal.pdf"),
        Path.Combine(Directory.GetCurrentDirectory(), "TestAssets", "sample-legal.pdf")
    };

    foreach (var candidate in candidates)
    {
        var fullPath = Path.GetFullPath(candidate);
        if (File.Exists(fullPath))
        {
            return fullPath;
        }
    }

    return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "TestAssets", "sample-legal.pdf"));
}
