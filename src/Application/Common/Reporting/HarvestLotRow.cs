using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed record HarvestLotRow(DateTimeOffset HarvestedAt, string LotCode, decimal Quantity, string Unit, string? Notes);
