using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour;

public sealed record RecordLabourCostRequest(Guid? Id, Guid? TransactionId, Guid CostCentreId,
    Guid? FieldOperationId, Guid? CropZoneId, Guid? LivestockSubjectId, string WorkerReference,
    DateTimeOffset OccurredAt, decimal DurationHours, decimal HourlyRate, string Currency,
    string Reference, string? Notes);
