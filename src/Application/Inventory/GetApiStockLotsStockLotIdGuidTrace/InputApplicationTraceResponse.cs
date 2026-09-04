using EdgIQ.MurimiOS.Domain.CropZone;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
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

public sealed record InputApplicationTraceResponse(
    Guid MovementId,
    Guid OperationId,
    Guid CropZoneId,
    string CropZone,
    string OperationType,
    decimal Quantity,
    DateTimeOffset OccurredAt);
