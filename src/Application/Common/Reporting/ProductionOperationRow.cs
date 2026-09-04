using System.Globalization;
using System.Text;

namespace EdgIQ.MurimiOS.Application.Common.Reporting;

public sealed record ProductionOperationRow(DateTimeOffset OccurredAt, string Type, string? Notes);
