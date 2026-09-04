using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.Shared;

public sealed record InputStockLotResponse(
    Guid Id,
    Guid InputId,
    string LotCode,
    DateOnly ReceivedOn,
    DateOnly? ExpiresOn,
    decimal InitialQuantity,
    decimal QuantityOnHand,
    string Unit,
    decimal EnteredInitialQuantity,
    string EnteredUnit,
    string DisplayUnit);
