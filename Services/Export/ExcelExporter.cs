using ClosedXML.Excel; // or EPPlus — add the NuGet package of your choice

namespace InventoryCRM.Services.Export;

public class ExcelExporter : IDocumentGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileExtension => ".xlsx";

    public Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "")
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(string.IsNullOrEmpty(title) ? "Sheet1" : title);

        var properties = typeof(T).GetProperties();

        // Header row
        for (int i = 0; i < properties.Length; i++)
            sheet.Cell(1, i + 1).Value = properties[i].Name;

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            for (int col = 0; col < properties.Length; col++)
                sheet.Cell(row, col + 1).Value = properties[col].GetValue(item)?.ToString() ?? "";
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}