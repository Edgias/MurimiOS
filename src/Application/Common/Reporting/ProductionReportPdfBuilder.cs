using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed class ProductionReportPdfBuilder
{
    public byte[] Build(ProductionReportData report)
    {
        var document = new PdfLayout(report.ReportId, report.GeneratedAt);
        document.Section("Production context");
        document.KeyValues([
            ("Farm", report.Farm),
            ("Season", report.Season),
            ("Field", report.Field),
            ("Crop zone", report.CropZone),
            ("Crop", report.Crop),
            ("Variety", report.Variety ?? "Not recorded"),
            ("Measured area", $"{report.AreaHa:0.####} ha"),
            ("Boundary version", report.BoundaryVersionId.ToString())
        ]);

        document.Section("Input applications");
        document.Table(
            ["Date", "Input and supplier lot", "Quantity", "Notes"],
            [78, 182, 72, 179],
            report.InputApplications.Select(item => new[]
            {
                FormatDate(item.OccurredAt),
                $"{item.Input}\nLot {item.StockLot}",
                $"{item.Quantity:0.####} {item.Unit}",
                item.Notes ?? string.Empty
            }),
            "Input applications");

        document.Section("Field operations");
        document.Table(
            ["Date", "Operation", "Notes"],
            [88, 132, 291],
            report.Operations.Select(item => new[]
            {
                FormatDate(item.OccurredAt),
                item.Type,
                item.Notes ?? string.Empty
            }),
            "Field operations");

        document.Section("Harvest lots");
        document.Table(
            ["Harvested", "Lot code", "Quantity", "Notes"],
            [88, 130, 80, 213],
            report.HarvestLots.Select(item => new[]
            {
                FormatDate(item.HarvestedAt),
                item.LotCode,
                $"{item.Quantity:0.####} {item.Unit}",
                item.Notes ?? string.Empty
            }),
            "Harvest lots");

        document.Section("Photo and GPS evidence");
        document.Table(
            ["Captured", "File", "GPS", "Integrity"],
            [88, 148, 142, 133],
            report.Evidence.Select(item => new[]
            {
                FormatDate(item.CapturedAt),
                $"{item.FileName}\n{FormatBytes(item.ByteSize)} {item.ContentType}",
                FormatLocation(item),
                $"ID {item.Id.ToString()[..8]}\nSHA-256 {item.Sha256[..12]}..."
            }),
            "Photo and GPS evidence");

        document.Section("Report scope and limitations");
        document.Paragraph(
            "This report reflects tenant records available at generation time. Harvest lots are associated through the crop zone. " +
            "It is an operational traceability record, not certification against a named standard.");
        return document.Render();
    }

    private static string FormatDate(DateTimeOffset value) =>
        value.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);

    private static string FormatBytes(long bytes) => bytes >= 1024 * 1024
        ? $"{bytes / 1024d / 1024d:0.##} MB"
        : $"{bytes / 1024d:0.##} KB";

    private static string FormatLocation(EvidenceRow evidence)
    {
        if (evidence.Latitude is null || evidence.Longitude is null)
        {
            return "Not captured";
        }

        string accuracy = evidence.AccuracyMetres is { } metres ? $" +/- {metres:0.#} m" : string.Empty;
        return $"{evidence.Latitude:0.#####}, {evidence.Longitude:0.#####}{accuracy}";
    }

    private sealed class PdfLayout(Guid reportId, DateTimeOffset generatedAt)
    {
        private const float PageWidth = 595;
        private const float PageHeight = 842;
        private const float Margin = 42;
        private const float ContentWidth = PageWidth - 2 * Margin;
        private readonly List<StringBuilder> pages = [];
        private StringBuilder page = null!;
        private float y;

        public void Section(string title)
        {
            EnsureSpace(34);
            FillRect(Margin, y, 4, 18, 0.72f, 0.24f, 0.12f);
            Text(Margin + 12, y + 1, title, 15, bold: true, 0.08f, 0.23f, 0.16f);
            y += 29;
        }

        public void KeyValues(IReadOnlyList<(string Label, string Value)> values)
        {
            const float columnGap = 20;
            float columnWidth = (ContentWidth - columnGap) / 2;
            for (int index = 0; index < values.Count; index += 2)
            {
                EnsureSpace(38);
                DrawKeyValue(Margin, y, columnWidth, values[index]);
                if (index + 1 < values.Count)
                {
                    DrawKeyValue(Margin + columnWidth + columnGap, y, columnWidth, values[index + 1]);
                }

                y += 36;
            }

            y += 4;
        }

        public void Paragraph(string value)
        {
            IReadOnlyList<string> lines = Wrap(value, ContentWidth, 10);
            EnsureSpace(lines.Count * 14 + 18, 807);
            FillRect(Margin, y, ContentWidth, lines.Count * 14 + 16, 0.96f, 0.95f, 0.91f);
            float lineY = y + 12;
            foreach (string line in lines)
            {
                Text(Margin + 12, lineY, line, 10, false, 0.20f, 0.22f, 0.19f);
                lineY += 14;
            }

            y += lines.Count * 14 + 24;
        }

        public void Table(
            IReadOnlyList<string> headers,
            IReadOnlyList<float> widths,
            IEnumerable<string[]> rows,
            string continuationTitle)
        {
            string[][] materialized = rows.ToArray();
            DrawTableHeader(headers, widths);
            if (materialized.Length == 0)
            {
                EnsureSpace(30);
                FillRect(Margin, y, ContentWidth, 26, 0.98f, 0.98f, 0.96f);
                Text(Margin + 10, y + 8, "No records available for this section.", 9, false, 0.42f, 0.43f, 0.39f);
                y += 34;
                return;
            }

            int rowIndex = 0;
            foreach (string[] row in materialized)
            {
                List<string>[] wrapped = row
                    .Select((cell, index) => WrapCell(cell, widths[index], 8.5f).ToList())
                    .ToArray();
                int lineCount = wrapped.Max(cell => cell.Count);
                float rowHeight = Math.Max(25, lineCount * 11 + 10);
                if (y + rowHeight > 780)
                {
                    NewPage();
                    Text(Margin, y, $"{continuationTitle} - continued", 11, true, 0.45f, 0.15f, 0.09f);
                    y += 20;
                    DrawTableHeader(headers, widths);
                }

                if (rowIndex % 2 == 1)
                {
                    FillRect(Margin, y, ContentWidth, rowHeight, 0.975f, 0.972f, 0.955f);
                }

                float x = Margin;
                for (int column = 0; column < wrapped.Length; column++)
                {
                    float lineY = y + 8;
                    foreach (string line in wrapped[column])
                    {
                        Text(x + 6, lineY, line, 8.5f, false, 0.15f, 0.17f, 0.14f);
                        lineY += 11;
                    }

                    x += widths[column];
                }

                StrokeLine(Margin, y + rowHeight, Margin + ContentWidth, y + rowHeight, 0.86f, 0.85f, 0.80f);
                y += rowHeight;
                rowIndex++;
            }

            y += 8;
        }

        public byte[] Render()
        {
            if (pages.Count == 0)
            {
                NewPage();
            }

            var contents = new List<byte[]>(pages.Count);
            for (int index = 0; index < pages.Count; index++)
            {
                var commands = new StringBuilder(pages[index].ToString());
                commands.Append("0.62 0.63 0.58 rg\n");
                commands.Append(TextCommand(Margin, 817, $"MurimiOS | Report {reportId} | Page {index + 1} of {pages.Count}", 8, false));
                contents.Add(Encoding.ASCII.GetBytes(commands.ToString()));
            }

            return PdfWriter.Write(contents);
        }

        private void NewPage()
        {
            page = new StringBuilder();
            pages.Add(page);
            FillRect(0, 0, PageWidth, 94, 0.055f, 0.22f, 0.145f);
            Text(Margin, 27, "MURIMIOS", 10, true, 0.76f, 0.84f, 0.48f);
            Text(Margin, 47, "Production and evidence report", 23, true, 1, 1, 1);
            Text(Margin, 73, $"Generated {generatedAt.UtcDateTime:yyyy-MM-dd HH:mm 'UTC'}", 9, false, 0.82f, 0.87f, 0.83f);
            Text(PageWidth - Margin - 190, 73, $"Report {reportId.ToString()[..8]}", 9, false, 0.82f, 0.87f, 0.83f);
            y = 116;
        }

        private void DrawKeyValue(float x, float top, float width, (string Label, string Value) value)
        {
            Text(x, top, value.Label.ToUpperInvariant(), 7.5f, true, 0.45f, 0.45f, 0.40f);
            IReadOnlyList<string> lines = Wrap(value.Value, width, 10.5f);
            Text(x, top + 13, lines.FirstOrDefault() ?? string.Empty, 10.5f, true, 0.11f, 0.22f, 0.16f);
            StrokeLine(x, top + 30, x + width, top + 30, 0.86f, 0.85f, 0.80f);
        }

        private void DrawTableHeader(IReadOnlyList<string> headers, IReadOnlyList<float> widths)
        {
            EnsureSpace(31);
            FillRect(Margin, y, ContentWidth, 25, 0.89f, 0.88f, 0.81f);
            float x = Margin;
            for (int index = 0; index < headers.Count; index++)
            {
                Text(x + 6, y + 8, headers[index].ToUpperInvariant(), 7.5f, true, 0.26f, 0.27f, 0.23f);
                x += widths[index];
            }

            y += 25;
        }

        private void EnsureSpace(float required, float maximumY = 780)
        {
            if (pages.Count == 0 || y + required > maximumY)
            {
                NewPage();
            }
        }

        private void Text(float x, float top, string value, float size, bool bold, float red, float green, float blue)
        {
            page.AppendFormat(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} rg\n", red, green, blue);
            page.Append(TextCommand(x, top, value, size, bold));
        }

        private static string TextCommand(float x, float top, string value, float size, bool bold)
        {
            float baseline = PageHeight - top - size;
            return FormattableString.Invariant(
                $"BT /{(bold ? "F2" : "F1")} {size:0.##} Tf {x:0.##} {baseline:0.##} Td ({Escape(value)}) Tj ET\n");
        }

        private void FillRect(float x, float top, float width, float height, float red, float green, float blue)
        {
            float bottom = PageHeight - top - height;
            page.AppendFormat(
                CultureInfo.InvariantCulture,
                "{0:0.###} {1:0.###} {2:0.###} rg {3:0.##} {4:0.##} {5:0.##} {6:0.##} re f\n",
                red,
                green,
                blue,
                x,
                bottom,
                width,
                height);
        }

        private void StrokeLine(float x1, float top1, float x2, float top2, float red, float green, float blue)
        {
            page.AppendFormat(
                CultureInfo.InvariantCulture,
                "{0:0.###} {1:0.###} {2:0.###} RG 0.5 w {3:0.##} {4:0.##} m {5:0.##} {6:0.##} l S\n",
                red,
                green,
                blue,
                x1,
                PageHeight - top1,
                x2,
                PageHeight - top2);
        }

        private static IReadOnlyList<string> Wrap(string value, float width, float fontSize) =>
            WrapCell(value, width, fontSize).ToArray();

        private static IEnumerable<string> WrapCell(string value, float width, float fontSize)
        {
            int maximumCharacters = Math.Max(5, (int)(width / (fontSize * 0.53f)));
            foreach (string explicitLine in (value ?? string.Empty).Replace("\r", string.Empty).Split('\n'))
            {
                string[] words = explicitLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length == 0)
                {
                    yield return string.Empty;
                    continue;
                }

                var line = new StringBuilder();
                foreach (string word in words)
                {
                    if (line.Length > 0 && line.Length + word.Length + 1 > maximumCharacters)
                    {
                        yield return line.ToString();
                        line.Clear();
                    }

                    if (line.Length > 0)
                    {
                        line.Append(' ');
                    }

                    if (word.Length <= maximumCharacters)
                    {
                        line.Append(word);
                    }
                    else
                    {
                        int offset = 0;
                        while (offset < word.Length)
                        {
                            int length = Math.Min(maximumCharacters, word.Length - offset);
                            if (line.Length > 0)
                            {
                                yield return line.ToString();
                                line.Clear();
                            }

                            string segment = word.Substring(offset, length);
                            offset += length;
                            if (offset < word.Length)
                            {
                                yield return segment;
                            }
                            else
                            {
                                line.Append(segment);
                            }
                        }
                    }
                }

                if (line.Length > 0)
                {
                    yield return line.ToString();
                }
            }
        }

        private static string Escape(string value)
        {
            var result = new StringBuilder(value.Length);
            foreach (char character in value)
            {
                char normalized = character switch
                {
                    '\u2013' or '\u2014' or '\u2011' => '-',
                    >= ' ' and <= '~' => character,
                    _ => '?'
                };
                if (normalized is '\\' or '(' or ')')
                {
                    result.Append('\\');
                }

                result.Append(normalized);
            }

            return result.ToString();
        }
    }

    private static class PdfWriter
    {
        public static byte[] Write(IReadOnlyList<byte[]> pageContents)
        {
            int objectCount = 4 + pageContents.Count * 2;
            var objects = new byte[objectCount + 1][];
            objects[1] = Ascii("<< /Type /Catalog /Pages 2 0 R >>");
            string kids = string.Join(' ', Enumerable.Range(0, pageContents.Count).Select(index => $"{5 + index * 2} 0 R"));
            objects[2] = Ascii($"<< /Type /Pages /Kids [{kids}] /Count {pageContents.Count} >>");
            objects[3] = Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");
            objects[4] = Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");

            for (int index = 0; index < pageContents.Count; index++)
            {
                int pageObject = 5 + index * 2;
                int streamObject = pageObject + 1;
                objects[pageObject] = Ascii(
                    $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] " +
                    $"/Resources << /Font << /F1 3 0 R /F2 4 0 R >> >> /Contents {streamObject} 0 R >>");
                byte[] header = Ascii($"<< /Length {pageContents[index].Length} >>\nstream\n");
                byte[] footer = Ascii("\nendstream");
                objects[streamObject] = [.. header, .. pageContents[index], .. footer];
            }

            using var output = new MemoryStream();
            Write(output, "%PDF-1.4\n%MurimiOS\n");
            var offsets = new long[objectCount + 1];
            for (int objectNumber = 1; objectNumber <= objectCount; objectNumber++)
            {
                offsets[objectNumber] = output.Position;
                Write(output, $"{objectNumber} 0 obj\n");
                output.Write(objects[objectNumber]);
                Write(output, "\nendobj\n");
            }

            long xrefOffset = output.Position;
            Write(output, $"xref\n0 {objectCount + 1}\n");
            Write(output, "0000000000 65535 f \n");
            for (int objectNumber = 1; objectNumber <= objectCount; objectNumber++)
            {
                Write(output, $"{offsets[objectNumber]:D10} 00000 n \n");
            }

            Write(output, $"trailer\n<< /Size {objectCount + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");
            return output.ToArray();
        }

        private static byte[] Ascii(string value) => Encoding.ASCII.GetBytes(value);

        private static void Write(Stream stream, string value) => stream.Write(Ascii(value));
    }
}
