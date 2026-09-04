using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiStockLotsStockLotIdGuidTrace;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;

public sealed record GetApiStockLotsStockLotIdGuidTraceQuery(Guid stockLotId) : IRequest<UseCaseResult>;
