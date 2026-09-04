using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour;
public sealed record LabourCostResponse(Guid Id, Guid TransactionId, Guid FarmId, Guid CostCentreId,
    Guid? FieldOperationId, Guid? CropZoneId, Guid? LivestockSubjectId, string WorkerReference,
    DateTimeOffset OccurredAt, decimal DurationHours, decimal DurationMinutes, decimal HourlyRate,
    decimal TotalCost, string Currency, string? Notes);
