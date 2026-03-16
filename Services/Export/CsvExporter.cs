using System.Text;

namespace InventoryCRM.Services.Export;

public class CsvExporter : IDocumentGenerator
{
    public string ContentType => "text/csv";
    public string FileExtension => ".csv";

    public Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "")
    {
        var properties = typeof(T).GetProperties();
        var sb = new StringBuilder();

        // Header
        sb.AppendLine(string.Join(",", properties.Select(p => $"\"{p.Name}\"")));

        // Rows
        foreach (var item in data)
        {
            var values = properties.Select(p =>
            {
                var val = p.GetValue(item)?.ToString()?.Replace("\"", "\"\"") ?? "";
                return $"\"{val}\"";
            });
            sb.AppendLine(string.Join(",", values));
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}