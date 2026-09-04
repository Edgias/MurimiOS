using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots;


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
