using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Imports;
public sealed record ParsedMasterDataRow(int Ordinal, IReadOnlyDictionary<string, string> Values);
