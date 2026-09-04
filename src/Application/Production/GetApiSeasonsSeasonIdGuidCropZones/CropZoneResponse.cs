using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiSeasonsSeasonIdGuidCropZones;


public sealed record CropZoneResponse(
    Guid Id,
    Guid SeasonId,
    Guid FieldId,
    Guid BoundaryVersionId,
    string Name,
    string Crop,
    string? Variety,
    decimal AreaHa);
