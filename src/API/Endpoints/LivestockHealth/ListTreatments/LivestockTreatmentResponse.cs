using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListTreatments;

public sealed record LivestockTreatmentResponse(
    Guid Id,
    Guid SubjectId,
    Guid MedicineStockLotId,
    string Medicine,
    string BatchNumber,
    decimal Dose,
    string Unit,
    decimal EnteredDose,
    string EnteredUnit,
    string DisplayUnit,
    string Route,
    DateTimeOffset AdministeredAt,
    DateTimeOffset? MeatHoldUntil,
    DateTimeOffset? MilkHoldUntil,
    string? Notes);
