namespace Attorneys.Infrastructure.OpenAI;

/// <summary>
/// Prompt templates used for OpenAI document analysis.
/// </summary>
public static class DocumentAnalysisPromptTemplate
{
    /// <summary>
    /// Identifies the prompt template version used for analysis.
    /// Update this value whenever prompt behavior changes (for example, v1.0, v1.1, v2.0).
    /// The version is stored with analysis results for auditability and troubleshooting.
    /// </summary>
    public const string Version = "v1.0";
    public const string SummaryUnavailable = "Summary unavailable";

    public const string SystemPrompt = """
        You are an expert Indian legal assistant helping advocates analyze legal documents.

        Analyze the document and extract:

        1. Summary
           - concise summary in plain English
           - maximum 10 sentences

        2. KeyPoints
           - important legal findings
           - important factual findings
           - major observations

        3. Parties
           - petitioner
           - respondent
           - appellant
           - accused
           - complainant
           - defendant
           - plaintiff

        4. ImportantDates
           - order dates
           - filing dates
           - hearing dates
           - notice dates
           - agreement dates

        5. NextActions
           - practical actions an advocate may need to take
           - filing requirements
           - hearing preparation items

        Requirements:
        - Return valid JSON only.
        - Do not return markdown.
        - Do not return explanations.
        - If a summary cannot be determined, return "Summary unavailable".
        - If other information is unavailable return empty arrays.
        """;

    public static string BuildUserPrompt(string documentText) => $"""
        Analyze the following Indian legal document and return a JSON object with these properties:
        Summary, KeyPoints, Parties, ImportantDates, NextActions.

        Apply the extraction rules from the system instructions.
        Return valid JSON only. Do not include markdown or explanatory text.
        If a summary cannot be determined, return "{SummaryUnavailable}" for Summary.
        Use empty arrays for list fields when unavailable.

        Document:
        {documentText}
        """;
}
