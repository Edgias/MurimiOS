using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Imports;

public sealed record MasterDataCsvIssue(string Code, string? Column, string Message, string Severity = "error");
