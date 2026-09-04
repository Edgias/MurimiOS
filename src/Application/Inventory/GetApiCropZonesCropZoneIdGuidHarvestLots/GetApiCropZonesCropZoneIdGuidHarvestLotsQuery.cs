using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots;


public sealed record GetApiCropZonesCropZoneIdGuidHarvestLotsQuery(Guid cropZoneId) : IRequest<UseCaseResult>;
