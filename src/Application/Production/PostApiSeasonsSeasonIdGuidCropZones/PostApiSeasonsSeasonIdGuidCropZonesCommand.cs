using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

public sealed record PostApiSeasonsSeasonIdGuidCropZonesCommand(Guid seasonId, CreateCropZoneRequest request) : IRequest<UseCaseResult>;
