using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery;

public sealed record RecordMachineryCostRequest(Guid? Id, Guid? TransactionId, Guid CostCentreId,
    Guid? FieldOperationId, Guid? CropZoneId, Guid? LivestockSubjectId, string MachineReference,
    DateTimeOffset OccurredAt, decimal DurationHours, decimal HourlyRate, decimal AdditionalCost,
    string Currency, string Reference, string? Notes);
