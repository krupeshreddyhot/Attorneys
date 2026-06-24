using System.Text;
using Attorneys.Application.Common.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;

namespace Attorneys.Infrastructure.Services;

public class DocumentTextExtractionService : IDocumentTextExtractionService
{
    public async Task<string> ExtractTextAsync(Stream stream, string fileName, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrEmpty(extension))
        {
            throw new NotSupportedException(
                $"Cannot determine file format from file name '{fileName}'. Supported formats: PDF, DOCX, TXT.");
        }

        MemoryStream? ownedStream = null;
        Stream workStream;

        try
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
                workStream = stream;
            }
            else
            {
                ownedStream = new MemoryStream();
                await stream.CopyToAsync(ownedStream, cancellationToken);
                ownedStream.Position = 0;
                workStream = ownedStream;
            }

            return extension.ToLowerInvariant() switch
            {
                ".pdf" => ExtractFromPdf(workStream, cancellationToken),
                ".docx" => ExtractFromDocx(workStream, cancellationToken),
                ".txt" => await ExtractFromTxtAsync(workStream, cancellationToken),
                _ => throw new NotSupportedException(
                    $"Text extraction is not supported for '{extension}' files. Supported formats: PDF, DOCX, TXT.")
            };
        }
        finally
        {
            ownedStream?.Dispose();
        }
    }

    private static string ExtractFromPdf(Stream stream, CancellationToken cancellationToken)
    {
        using var document = PdfDocument.Open(stream);
        var text = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();
            text.AppendLine(page.Text);
        }

        return text.ToString().TrimEnd();
    }

    private static string ExtractFromDocx(Stream stream, CancellationToken cancellationToken)
    {
        using var document = WordprocessingDocument.Open(stream, false);
        var body = document.MainDocumentPart?.Document?.Body;
        if (body is null)
        {
            return string.Empty;
        }

        var text = new StringBuilder();
        foreach (var paragraph in body.Descendants<Paragraph>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var paragraphText = string.Concat(paragraph.Descendants<Text>().Select(t => t.Text));
            if (paragraphText.Length > 0)
            {
                text.AppendLine(paragraphText);
            }
        }

        return text.ToString().TrimEnd();
    }

    private static async Task<string> ExtractFromTxtAsync(Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
        return await reader.ReadToEndAsync(cancellationToken);
    }
}
