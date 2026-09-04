using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListSoilZones;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;

public sealed record ListSoilZonesQuery(Guid farmId, Guid? fieldId) : IRequest<UseCaseResult>;
