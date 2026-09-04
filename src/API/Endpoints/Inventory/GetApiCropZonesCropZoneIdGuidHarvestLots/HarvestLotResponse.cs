using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots;

public sealed record HarvestLotResponse(
    Guid Id,
    Guid CropZoneId,
    string LotCode,
    decimal Quantity,
    string Unit,
    decimal EnteredQuantity,
    string EnteredUnit,
    string DisplayUnit,
    DateTimeOffset HarvestedAt,
    string? Notes);
