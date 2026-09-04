using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Imports;
public sealed record MasterDataCsvParseResult(IReadOnlyList<string> Header,
    IReadOnlyList<ParsedMasterDataRow> Rows, IReadOnlyList<MasterDataCsvIssue> FileIssues);
