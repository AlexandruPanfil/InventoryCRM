namespace InventoryCRM.Services.Export;

public interface IDocumentGenerator
{
    /// <summary>
    /// Generates a document from the given data and returns it as a byte array.
    /// </summary>
    Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "");

    /// <summary>
    /// The MIME content type for the generated file (e.g. "application/pdf").
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// The default file extension (e.g. ".pdf").
    /// </summary>
    string FileExtension { get; }
}