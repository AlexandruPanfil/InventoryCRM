using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using InventoryCRM.Models;

namespace InventoryCRM.Services.Export;

public class PdfExporter : IDocumentGenerator
{
    public string ContentType => "application/pdf";
    public string FileExtension => ".pdf";

    // ── Generic fallback (keeps the interface happy) ──────────────────────
    public Task<byte[]> GenerateAsync<T>(IEnumerable<T> data, string title = "")
    {
        var properties = typeof(T).GetProperties();
        var rows = data.ToList();

        using var document = new PdfDocument();
        var page = document.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        using var gfx = XGraphics.FromPdfPage(page);

        var fontBold = new XFont("Arial", 10, XFontStyle.Bold);
        var fontNormal = new XFont("Arial", 9, XFontStyle.Regular);

        double x = 40, y = 40;
        double colWidth = (page.Width - 80) / properties.Length;

        // Title
        gfx.DrawString(title, new XFont("Arial", 13, XFontStyle.Bold),
            XBrushes.Black, new XPoint(x, y));
        y += 24;

        // Header row
        foreach (var prop in properties)
        {
            gfx.DrawString(prop.Name, fontBold, XBrushes.Black, new XPoint(x, y));
            x += colWidth;
        }
        y += 18;

        // Data rows
        foreach (var item in rows)
        {
            x = 40;
            foreach (var prop in properties)
            {
                var val = prop.GetValue(item)?.ToString() ?? "";
                gfx.DrawString(val, fontNormal, XBrushes.Black, new XPoint(x, y));
                x += colWidth;
            }
            y += 16;
        }

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return Task.FromResult(stream.ToArray());
    }

    // ── Dedicated Order export ────────────────────────────────────────────
    public Task<byte[]> GenerateOrderAsync(Order order)
    {
        using var document = new PdfDocument();
        document.Info.Title = $"Order {order.Identifier}";

        var page = document.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        using var gfx = XGraphics.FromPdfPage(page);

        double pageW = page.Width;
        double marginX = 40;
        double contentW = pageW - marginX * 2;
        double y = 40;

        // ── Fonts & brushes ───────────────────────────────────────────────
        var fontTitle = new XFont("Arial", 16, XFontStyle.Bold);
        var fontSection = new XFont("Arial", 10, XFontStyle.Bold);
        var fontLabel = new XFont("Arial", 9, XFontStyle.Bold);
        var fontValue = new XFont("Arial", 9, XFontStyle.Regular);

        var brushWhite = XBrushes.White;
        var brushBlack = XBrushes.Black;
        var brushDarkBlue = new XSolidBrush(XColor.FromArgb(31, 78, 121));
        var brushLightBlue = new XSolidBrush(XColor.FromArgb(214, 228, 240));
        var brushZebra = new XSolidBrush(XColor.FromArgb(235, 245, 251));
        var brushTableHead = new XSolidBrush(XColor.FromArgb(46, 117, 182));

        double rowH = 18;

        // ── Title ─────────────────────────────────────────────────────────
        gfx.DrawString($"Order {order.Identifier}", fontTitle,
            brushBlack, new XPoint(marginX, y));
        y += 30;

        // ── Section header helper ─────────────────────────────────────────
        void DrawSectionHeader(string text)
        {
            gfx.DrawRectangle(brushDarkBlue,
                new XRect(marginX, y - 13, contentW, rowH));
            gfx.DrawString(text, fontSection, brushWhite,
                new XPoint(marginX + 4, y));
            y += rowH + 2;
        }

        // ── Label/value row helper ────────────────────────────────────────
        void DrawInfoRow(string label, string value, bool shaded)
        {
            double labelW = 120;
            if (shaded)
                gfx.DrawRectangle(brushLightBlue,
                    new XRect(marginX, y - 13, labelW, rowH));

            gfx.DrawString(label, fontLabel, brushBlack, new XPoint(marginX + 4, y));
            gfx.DrawString(value, fontValue, brushBlack, new XPoint(marginX + labelW + 8, y));
            y += rowH;
        }

        // ── Order Information ─────────────────────────────────────────────
        DrawSectionHeader("Order Information");

        var fields = new[]
        {
            ("Order #",        order.Identifier),
            ("Status",         order.Status ?? ""),
            ("Customer",       order.Customers?.Name ?? ""),
            ("Worker",         order.Worker?.Workername ?? ""),
            ("Description",    order.Description ?? ""),
            ("Created",        order.CreatedAt.ToLocalTime().ToString("g")),
            ("Last Updated",   order.UpdatedAt.ToLocalTime().ToString("g")),
            ("Schedule Start", order.Schedule?.StartTime.ToLocalTime().ToString("g") ?? "—"),
            ("Schedule End",   order.Schedule?.EndTime.ToLocalTime().ToString("g")   ?? "—"),
        };

        for (int i = 0; i < fields.Length; i++)
            DrawInfoRow(fields[i].Item1, fields[i].Item2, i % 2 == 0);

        y += 12; // spacer

        // ── Assigned Units ────────────────────────────────────────────────
        DrawSectionHeader("Assigned Units");

        // Table column widths
        double col1 = contentW * 0.55;
        double col2 = contentW * 0.20;
        double col3 = contentW * 0.25;

        // Table header row
        gfx.DrawRectangle(brushTableHead,
            new XRect(marginX, y - 13, contentW, rowH));
        gfx.DrawString("Unit Name", fontSection, brushWhite,
            new XPoint(marginX + 4, y));
        gfx.DrawString("Quantity", fontSection, brushWhite,
            new XPoint(marginX + col1 + 4, y));
        gfx.DrawString("Status", fontSection, brushWhite,
            new XPoint(marginX + col1 + col2 + 4, y));
        y += rowH + 2;

        var unitList = order.UnitAssignment?.ToList() ?? new();
        for (int i = 0; i < unitList.Count; i++)
        {
            var u = unitList[i];
            if (i % 2 == 1)
                gfx.DrawRectangle(brushZebra,
                    new XRect(marginX, y - 13, contentW, rowH));

            gfx.DrawString(u.Name, fontValue, brushBlack, new XPoint(marginX + 4, y));
            gfx.DrawString(u.Quantity.ToString(), fontValue, brushBlack, new XPoint(marginX + col1 + 4, y));
            gfx.DrawString(u.Status ?? "", fontValue, brushBlack, new XPoint(marginX + col1 + col2 + 4, y));
            y += rowH;
        }

        // Total row
        y += 2;
        gfx.DrawRectangle(brushLightBlue,
            new XRect(marginX, y - 13, contentW, rowH));
        gfx.DrawLine(new XPen(XColor.FromArgb(31, 78, 121), 1),
            marginX, y - 14, marginX + contentW, y - 14);
        gfx.DrawString("TOTAL", fontLabel, brushBlack, new XPoint(marginX + 4, y));
        gfx.DrawString(unitList.Sum(u => u.Quantity).ToString(),
            fontLabel, brushBlack, new XPoint(marginX + col1 + 4, y));

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return Task.FromResult(stream.ToArray());
    }
}