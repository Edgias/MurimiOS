using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

public sealed record CropZoneResponse(
    Guid Id,
    Guid SeasonId,
    Guid FieldId,
    Guid BoundaryVersionId,
    string Name,
    string Crop,
    string? Variety,
    decimal AreaHa);
