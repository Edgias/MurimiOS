using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.Shared;

public sealed record VeterinaryMedicineStockLotResponse(
    Guid Id,
    Guid MedicineId,
    string BatchNumber,
    DateOnly ReceivedOn,
    DateOnly? ExpiresOn,
    decimal InitialQuantity,
    decimal QuantityOnHand,
    string Unit,
    decimal EnteredInitialQuantity,
    string EnteredUnit,
    string DisplayUnit);
