using ClosedXML.Excel;
using InventoryCRM.Models;

namespace InventoryCRM.Services.Export;

public class ExcelExporter : IDocumentGenerator
{
    public string ContentType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileExtension => ".xlsx";

    // ── Generic fallback (keeps the interface happy) ──────────────────────
    public Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "")
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add(string.IsNullOrEmpty(title) ? "Sheet1" : title);
        var properties = typeof(T).GetProperties();

        for (int i = 0; i < properties.Length; i++)
            sheet.Cell(1, i + 1).Value = properties[i].Name;

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

    // ── Dedicated Order export ─────────────────────────────────────────────
    public Task<byte[]> GenerateOrderAsync(Order order)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Order");

        // ── Helpers ───────────────────────────────────────────────────────
        void SectionHeader(int row, string text)
        {
            var cell = ws.Cell(row, 1);
            cell.Value = text;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E79");
            cell.Style.Font.FontColor = XLColor.White;
            ws.Range(row, 1, row, 3).Merge();
        }

        void LabelValue(int row, string label, string value)
        {
            var lc = ws.Cell(row, 1);
            lc.Value = label;
            lc.Style.Font.Bold = true;
            lc.Style.Fill.BackgroundColor = XLColor.FromHtml("#D6E4F0");

            var vc = ws.Cell(row, 2);
            vc.Value = value;
            ws.Range(row, 2, row, 3).Merge(); // value spans cols B-C
        }

        void TableHeader(int row, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(row, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        // ── Title ─────────────────────────────────────────────────────────
        var title = ws.Cell(1, 1);
        title.Value = $"Order {order.Identifier}";
        title.Style.Font.Bold = true;
        title.Style.Font.FontSize = 14;
        title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(1, 1, 1, 3).Merge();

        // ── Section: Order Info ───────────────────────────────────────────
        SectionHeader(3, "Order Information");

        var fields = new[]
        {
            ("Order #",         order.Identifier),
            ("Status",          order.Status ?? ""),
            ("Customer",        order.Customers?.Name ?? ""),
            ("Worker",          order.Worker?.Workername ?? ""),
            ("Description",     order.Description ?? ""),
            ("Created",         order.CreatedAt.ToLocalTime().ToString("g")),
            ("Last Updated",    order.UpdatedAt.ToLocalTime().ToString("g")),
            ("Schedule Start",  order.Schedule?.StartTime.ToLocalTime().ToString("g") ?? "—"),
            ("Schedule End",    order.Schedule?.EndTime.ToLocalTime().ToString("g")   ?? "—"),
        };

        int r = 4;
        foreach (var (label, value) in fields)
            LabelValue(r++, label, value);

        // ── Section: Units ────────────────────────────────────────────────
        r++;   // blank spacer row
        SectionHeader(r++, "Assigned Units");
        TableHeader(r++, new[] { "Unit Name", "Quantity", "Status" });

        var unitList = order.UnitAssignment?.ToList() ?? new();
        for (int i = 0; i < unitList.Count; i++)
        {
            var u = unitList[i];
            ws.Cell(r, 1).Value = u.Name;
            ws.Cell(r, 2).Value = u.Quantity;
            ws.Cell(r, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(r, 3).Value = u.Status ?? "";

            if (i % 2 == 1)
                ws.Range(r, 1, r, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#EBF5FB");

            r++;
        }

        // Total row
        ws.Cell(r, 1).Value = "TOTAL";
        ws.Cell(r, 1).Style.Font.Bold = true;
        ws.Cell(r, 2).Value = unitList.Sum(u => u.Quantity);
        ws.Cell(r, 2).Style.Font.Bold = true;
        ws.Cell(r, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(r, 1, r, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#D6E4F0");
        ws.Range(r, 1, r, 3).Style.Border.TopBorder = XLBorderStyleValues.Medium;

        // ── Column widths ─────────────────────────────────────────────────
        ws.Column(1).Width = 24;
        ws.Column(2).Width = 16;
        ws.Column(3).Width = 20;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}