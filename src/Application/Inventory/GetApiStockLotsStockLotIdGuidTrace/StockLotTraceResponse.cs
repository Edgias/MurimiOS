using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiStockLotsStockLotIdGuidTrace;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;

public sealed record StockLotTraceResponse(
    Guid StockLotId,
    string StockLotCode,
    string Input,
    string Unit,
    string DisplayUnit,
    decimal QuantityOnHand,
    IReadOnlyList<InputApplicationTraceResponse> Applications,
    IReadOnlyList<HarvestTraceResponse> RelatedHarvestLots);
