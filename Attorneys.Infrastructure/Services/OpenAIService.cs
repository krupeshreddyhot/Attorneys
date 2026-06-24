using Attorneys.Application.Common.Interfaces;
using Attorneys.Application.DTOs.AI;
using Attorneys.Infrastructure.Common;
using Attorneys.Infrastructure.OpenAI;
using Attorneys.Infrastructure.OpenAI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace Attorneys.Infrastructure.Services;

public class OpenAIService : IOpenAIService
{
    private const string JsonSchemaFormatName = "document_analysis";

    private static readonly BinaryData JsonSchema = BinaryData.FromBytes("""
        {
            "type": "object",
            "properties": {
                "Summary": {
                    "type": "string",
                    "minLength": 1
                },
                "KeyPoints": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "Parties": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "ImportantDates": {
                    "type": "array",
                    "items": { "type": "string" }
                },
                "NextActions": {
                    "type": "array",
                    "items": { "type": "string" }
                }
            },
            "required": ["Summary", "KeyPoints", "Parties", "ImportantDates", "NextActions"],
            "additionalProperties": false
        }
        """u8.ToArray());

    private readonly ChatClient _chatClient;
    private readonly string _model;
    private readonly int _maxCharacters;
    private readonly decimal _inputPerMillionTokens;
    private readonly decimal _outputPerMillionTokens;
    private readonly float _temperature;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IConfiguration configuration, ILogger<OpenAIService> logger)
    {
        _logger = logger;
        _model = OpenAISettings.GetModel(configuration);
        _maxCharacters = OpenAISettings.GetMaxCharacters(configuration);
        _inputPerMillionTokens = OpenAISettings.GetInputPerMillionTokens(configuration);
        _outputPerMillionTokens = OpenAISettings.GetOutputPerMillionTokens(configuration);
        _temperature = OpenAISettings.GetTemperature(configuration);

        var apiKey = OpenAISettings.GetApiKey(configuration)
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");

        _chatClient = new ChatClient(_model, apiKey);
    }

    public async Task<DocumentAnalysisResult> AnalyzeDocumentAsync(
        string documentText,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(documentText))
        {
            throw new ArgumentException("Document text is required.", nameof(documentText));
        }

        var preparedText = DocumentTextTruncator.TruncateIfNeeded(documentText, _maxCharacters, _logger);
        var request = DocumentAnalysisRequest.Create(preparedText);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(request.SystemPrompt),
            new UserChatMessage(request.UserPrompt)
        };

        var options = new ChatCompletionOptions
        {
            Temperature = _temperature,
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: JsonSchemaFormatName,
                jsonSchema: JsonSchema,
                jsonSchemaIsStrict: true)
        };

        var completion = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
        var responseText = completion.Value.Content.Count > 0
            ? completion.Value.Content[0].Text
            : string.Empty;

        var inputTokens = completion.Value.Usage?.InputTokenCount ?? 0;
        var outputTokens = completion.Value.Usage?.OutputTokenCount ?? 0;
        var estimatedCost = OpenAICostEstimator.Estimate(
            inputTokens,
            outputTokens,
            _inputPerMillionTokens,
            _outputPerMillionTokens,
            _logger);

        return DocumentAnalysisResponseParser.Parse(
            responseText,
            _model,
            DocumentAnalysisPromptTemplate.Version,
            inputTokens,
            outputTokens,
            estimatedCost,
            _logger);
    }
}
