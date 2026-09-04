using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiSeasonsSeasonIdGuidCropZones;


public sealed record GetApiSeasonsSeasonIdGuidCropZonesQuery(Guid seasonId) : IRequest<UseCaseResult>;
