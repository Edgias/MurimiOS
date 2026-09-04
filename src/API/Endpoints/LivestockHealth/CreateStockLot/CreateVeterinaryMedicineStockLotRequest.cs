using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateStockLot;

public sealed record CreateVeterinaryMedicineStockLotRequest(
    string BatchNumber,
    DateOnly ReceivedOn,
    DateOnly? ExpiresOn,
    decimal InitialQuantity);
