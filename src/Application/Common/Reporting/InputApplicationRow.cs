using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed record InputApplicationRow(
    DateTimeOffset OccurredAt,
    string Input,
    string StockLot,
    decimal Quantity,
    string Unit,
    string? Notes);
