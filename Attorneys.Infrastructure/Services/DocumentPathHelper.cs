namespace Attorneys.Infrastructure.Services;

internal static class DocumentPathHelper
{
    public static string SanitizeCaseNo(string value) =>
        string.Concat(value.Where(c => char.IsLetterOrDigit(c) || c is '-' or '_')).Trim();

    public static string SanitizeFileName(string fileName) =>
        string.Concat(Path.GetFileName(fileName).Where(c => !Path.GetInvalidFileNameChars().Contains(c)));

    public static string ContentTypeFromFileName(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".txt" => "text/plain",
            _ => "application/octet-stream"
        };
    }
}
