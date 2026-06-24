using Microsoft.Extensions.Logging;

namespace Attorneys.Infrastructure.OpenAI;

public static class DocumentTextTruncator
{
    public const string TruncationMarker = "[DOCUMENT TRUNCATED]";

    public static string TruncateIfNeeded(
        string documentText,
        int maxCharacters,
        ILogger logger)
    {
        if (documentText.Length <= maxCharacters)
        {
            return documentText;
        }

        var separator = Environment.NewLine + Environment.NewLine;
        var reservedLength = TruncationMarker.Length + (2 * separator.Length);
        var remainingCharacters = maxCharacters - reservedLength;

        var headLength = remainingCharacters > 0 ? remainingCharacters / 2 : 0;
        var tailLength = remainingCharacters > 0 ? remainingCharacters - headLength : 0;
        var head = headLength > 0 ? documentText[..headLength] : string.Empty;
        var tail = tailLength > 0 ? documentText[^tailLength..] : string.Empty;

        string truncated;
        if (remainingCharacters <= 0)
        {
            truncated = TruncationMarker.Length <= maxCharacters
                ? TruncationMarker
                : TruncationMarker[..maxCharacters];
        }
        else
        {
            truncated = string.Join(separator, head, TruncationMarker, tail);
        }

        var originalLength = documentText.Length;
        var finalLength = truncated.Length;
        var charactersRemoved = originalLength - finalLength;

        logger.LogWarning(
            "Document text length {OriginalLength} exceeded limit {MaxCharacters}. FinalLength={FinalLength} CharactersRemoved={CharactersRemoved}",
            originalLength,
            maxCharacters,
            finalLength,
            charactersRemoved);

        return truncated;
    }
}
