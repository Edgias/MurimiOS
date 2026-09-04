using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.PostApiInputsInputIdGuidStockLots;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

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
