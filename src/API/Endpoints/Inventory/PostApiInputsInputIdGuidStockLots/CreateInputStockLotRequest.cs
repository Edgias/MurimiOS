using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiInputsInputIdGuidStockLots;

public sealed record CreateInputStockLotRequest(
    string LotCode,
    DateOnly ReceivedOn,
    DateOnly? ExpiresOn,
    decimal InitialQuantity);
