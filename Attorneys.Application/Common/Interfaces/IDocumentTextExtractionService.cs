namespace Attorneys.Application.Common.Interfaces;

public interface IDocumentTextExtractionService
{
    Task<string> ExtractTextAsync(Stream stream, string fileName, CancellationToken cancellationToken);
}
