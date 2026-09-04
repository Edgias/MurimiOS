using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiCropZonesCropZoneIdGuidOperations;


public sealed record GetApiCropZonesCropZoneIdGuidOperationsQuery(Guid cropZoneId) : IRequest<UseCaseResult>;
