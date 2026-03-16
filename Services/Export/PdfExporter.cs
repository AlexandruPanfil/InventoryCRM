using QuestPDF.Fluent; // add QuestPDF NuGet package
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InventoryCRM.Services.Export;

public class PdfExporter : IDocumentGenerator
{
    public string ContentType => "application/pdf";
    public string FileExtension => ".pdf";

    public Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "")
    {
        var properties = typeof(T).GetProperties();
        var rows = data.ToList();

        var bytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Header().Text(title).SemiBold().FontSize(16);

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        foreach (var _ in properties)
                            cols.RelativeColumn();
                    });

                    // Header
                    table.Header(header =>
                    {
                        foreach (var prop in properties)
                            header.Cell().Text(prop.Name).Bold();
                    });

                    // Rows
                    foreach (var item in rows)
                        foreach (var prop in properties)
                            table.Cell().Text(prop.GetValue(item)?.ToString() ?? "");
                });
            });
        }).GeneratePdf();

        return Task.FromResult(bytes);
    }
}