using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery;
public sealed record MachineryCostResponse(Guid Id, Guid TransactionId, Guid FarmId, Guid CostCentreId,
    Guid? FieldOperationId, Guid? CropZoneId, Guid? LivestockSubjectId, string MachineReference,
    DateTimeOffset OccurredAt, decimal DurationHours, decimal DurationMinutes, decimal HourlyRate,
    decimal AdditionalCost, decimal TotalCost, string Currency, string? Notes);
