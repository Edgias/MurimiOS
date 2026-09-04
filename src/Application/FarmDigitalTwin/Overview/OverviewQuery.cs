using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.Overview;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleStatus = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleStatus;
using SoilZoneStatus = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneStatus;

public sealed record OverviewQuery(Guid farmId) : IRequest<UseCaseResult>;
