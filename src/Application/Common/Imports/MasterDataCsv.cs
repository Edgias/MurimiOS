using System.Text;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;

namespace EdgIQ.MurimiOS.Application.Common.Imports;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;

using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;

public static class MasterDataCsv
{
    public const string Version = "master-csv-1.0.0";
    public const int MaximumRows = 20_000;

    private static readonly IReadOnlyDictionary<MasterDataImportType, string[]> Columns =
        new Dictionary<MasterDataImportType, string[]>
        {
            [MasterDataImportType.Farms] = ["name", "code", "country_code", "time_zone", "parent_organisation_node_id"],
            [MasterDataImportType.Fields] = ["farm_code", "name", "boundary_wkt", "valid_from", "source"],
            [MasterDataImportType.Livestock] = ["farm_code", "kind", "name", "species", "official_identifier"],
            [MasterDataImportType.Inventory] = ["farm_code", "input_name", "input_category", "unit", "lot_code", "received_on", "expires_on", "quantity"],
            [MasterDataImportType.Suppliers] = ["supplier_code", "name", "contact_name", "email", "phone"],
            [MasterDataImportType.OpeningBalances] = ["farm_code", "account_code", "account_name", "direction", "amount", "currency", "as_of", "reference"],
            [MasterDataImportType.FinanceTransactions] = ["farm_code", "entry_type", "category", "amount", "currency", "occurred_at", "reference", "counterparty", "description"]
        };

    private static readonly IReadOnlyDictionary<MasterDataImportType, string[]> Optional =
        new Dictionary<MasterDataImportType, string[]>
        {
            [MasterDataImportType.Farms] = ["parent_organisation_node_id"],
            [MasterDataImportType.Fields] = ["source"],
            [MasterDataImportType.Livestock] = ["official_identifier"],
            [MasterDataImportType.Inventory] = ["expires_on"],
            [MasterDataImportType.Suppliers] = ["phone"],
            [MasterDataImportType.OpeningBalances] = [],
            [MasterDataImportType.FinanceTransactions] = ["counterparty", "description"]
        };

    public static IReadOnlyList<string> TemplateColumns(MasterDataImportType type) =>
        Columns.TryGetValue(type, out string[]? columns) ? columns : throw new ArgumentOutOfRangeException(nameof(type));

    public static byte[] Template(MasterDataImportType type) => Encoding.UTF8.GetBytes(
        Write([TemplateColumns(type), Example(type)]));

    public static MasterDataCsvParseResult Parse(MasterDataImportType type, byte[] bytes)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        string text;
        try { text = new UTF8Encoding(false, true).GetString(bytes); }
        catch (DecoderFallbackException) { return new([], [], [new("invalid_utf8", null, "The CSV source must be UTF-8 encoded.")]); }
        if (text.StartsWith('\uFEFF')) text = text[1..];
        List<string[]> records;
        try { records = ReadRecords(text); }
        catch (FormatException exception) { return new([], [], [new("csv_syntax_invalid", null, exception.Message)]); }
        if (records.Count == 0) return new([], [], [new("csv_empty", null, "The CSV source is empty.")]);
        string[] header = records[0].Select(NormalizeHeader).ToArray();
        if (header.Any(string.IsNullOrEmpty)) return new(header, [], [new("csv_header_empty", null, "Every CSV column requires a header.")]);
        if (header.Distinct(StringComparer.Ordinal).Count() != header.Length)
            return new(header, [], [new("csv_header_duplicate", null, "CSV headers must be unique after normalization.")]);
        string[] required = Columns[type].Except(Optional[type], StringComparer.Ordinal).ToArray();
        string[] missing = required.Except(header, StringComparer.Ordinal).ToArray();
        if (missing.Length > 0)
            return new(header, [], [new("csv_header_missing", null, $"Missing required columns: {string.Join(", ", missing)}.")]);
        if (records.Count - 1 > MaximumRows)
            return new(header, [], [new("csv_row_limit", null, $"A CSV import cannot exceed {MaximumRows:N0} data rows.")]);

        var rows = new List<ParsedMasterDataRow>();
        for (int index = 1; index < records.Count; index++)
        {
            string[] record = records[index];
            if (record.All(string.IsNullOrWhiteSpace)) continue;
            if (record.Length > header.Length && record.Skip(header.Length).Any(x => !string.IsNullOrWhiteSpace(x)))
                return new(header, [], [new("csv_column_count", null, $"Row {index + 1} has more values than the header.")]);
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int column = 0; column < header.Length; column++)
                values[header[column]] = column < record.Length ? record[column].Trim() : "";
            rows.Add(new(index, values));
        }
        return new(header, rows, []);
    }

    public static string Write(IEnumerable<IEnumerable<string?>> rows)
    {
        var output = new StringBuilder();
        foreach (IEnumerable<string?> row in rows)
        {
            output.AppendJoin(',', row.Select(Escape));
            output.Append("\r\n");
        }
        return output.ToString();
    }

    public static bool IsOptional(MasterDataImportType type, string column) => Optional[type].Contains(column);

    private static List<string[]> ReadRecords(string text)
    {
        var records = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        bool quoted = false;
        for (int index = 0; index < text.Length; index++)
        {
            char character = text[index];
            if (quoted)
            {
                if (character == '"' && index + 1 < text.Length && text[index + 1] == '"') { field.Append('"'); index++; }
                else if (character == '"') quoted = false;
                else field.Append(character);
            }
            else if (character == '"')
            {
                if (field.Length != 0) throw new FormatException("A quote may only begin an empty CSV field.");
                quoted = true;
            }
            else if (character == ',') { row.Add(field.ToString()); field.Clear(); }
            else if (character is '\r' or '\n')
            {
                if (character == '\r' && index + 1 < text.Length && text[index + 1] == '\n') index++;
                row.Add(field.ToString()); field.Clear(); records.Add(row.ToArray()); row.Clear();
            }
            else field.Append(character);
            if (field.Length > 1_000_000) throw new FormatException("A CSV field exceeds the one-megabyte limit.");
        }
        if (quoted) throw new FormatException("The CSV ends inside a quoted field.");
        if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); records.Add(row.ToArray()); }
        while (records.Count > 0 && records[^1].All(string.IsNullOrWhiteSpace)) records.RemoveAt(records.Count - 1);
        return records;
    }

    private static string NormalizeHeader(string value) => value.Trim().ToLowerInvariant().Replace('-', '_').Replace(' ', '_');
    private static string Escape(string? value)
    {
        string result = value ?? "";
        return result.IndexOfAny([',', '"', '\r', '\n']) >= 0 ? $"\"{result.Replace("\"", "\"\"")}\"" : result;
    }

    private static string[] Example(MasterDataImportType type) => type switch
    {
        MasterDataImportType.Farms => ["Example Farm", "EXAMPLE-01", "ZW", "Africa/Harare", ""],
        MasterDataImportType.Fields => ["EXAMPLE-01", "North Block", "POLYGON ((31 -17.8, 31.01 -17.8, 31.01 -17.81, 31 -17.81, 31 -17.8))", "2026-09-01", "surveyed"],
        MasterDataImportType.Livestock => ["EXAMPLE-01", "Individual", "Cow 001", "Cattle", "ZW-001"],
        MasterDataImportType.Inventory => ["EXAMPLE-01", "Compound D", "Fertiliser", "kg", "LOT-001", "2026-09-01", "2027-09-01", "1000"],
        MasterDataImportType.Suppliers => ["SUP-001", "Example Inputs", "Supplier Contact", "supplier@example.test", "+263000000000"],
        MasterDataImportType.OpeningBalances => ["EXAMPLE-01", "1000", "Cash", "Debit", "5000", "USD", "2026-09-01", "OPEN-2026"],
        MasterDataImportType.FinanceTransactions => ["EXAMPLE-01", "Expense", "Seed", "1200", "USD", "2026-09-01T08:00:00Z", "INV-001", "Example Inputs", "Opening seed purchase"],
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}
