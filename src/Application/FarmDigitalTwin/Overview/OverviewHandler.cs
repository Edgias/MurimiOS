using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
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

public sealed class OverviewHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<OverviewQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(OverviewQuery message, CancellationToken cancellationToken) => OverviewAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> OverviewAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        Farm? farm = await db.Farms.AsNoTracking().SingleOrDefaultAsync(x => x.Id == farmId, ct);
        if (farm is null) return UseCaseResults.NotFound();
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        int fields = await db.FarmFields.CountAsync(x => x.FarmId == farmId, ct);
        int activeTenures = await db.LandTenureRecords.CountAsync(x => x.FarmId == farmId
            && x.ValidFrom <= today && (x.ValidTo == null || x.ValidTo >= today), ct);
        var infrastructure = await db.SiteInfrastructureAssets.AsNoTracking().Where(x => x.FarmId == farmId)
            .GroupBy(x => x.Status).Select(x => new { status = x.Key, count = x.Count() }).ToArrayAsync(ct);
        int activeSoilZones = await db.SoilZones.CountAsync(x => x.FarmId == farmId && x.Status == SoilZoneStatus.Active, ct);
        int soilSamples = await db.SoilSamples.CountAsync(x => x.FarmId == farmId, ct);
        int analysedSamples = await db.SoilSamples.CountAsync(x => x.FarmId == farmId && x.Status == SoilSampleStatus.Analysed, ct);
        DateTimeOffset? latestSample = await db.SoilSamples.Where(x => x.FarmId == farmId)
            .MaxAsync(x => (DateTimeOffset?)x.SampledAt, ct);
        SiteInfrastructureAsset[] mappedAssets = await db.SiteInfrastructureAssets.AsNoTracking()
            .Where(x => x.FarmId == farmId && x.Location != null && x.Status != InfrastructureAssetStatus.Decommissioned)
            .OrderBy(x => x.Code).ToArrayAsync(ct);
        object[] assetLayer = mappedAssets.Select(x => (object)new
            {
                type = "Feature", geometry = new { type = "Point", coordinates = new[] { x.Location!.X, x.Location.Y } },
                properties = new { x.Id, x.Code, x.Name, x.Type, x.Status, layer = "infrastructure" }
            }).ToArray();
        SoilSample[] mappedSamples = await db.SoilSamples.AsNoTracking().Where(x => x.FarmId == farmId)
            .OrderByDescending(x => x.SampledAt).Take(2_000).ToArrayAsync(ct);
        object[] sampleLayer = mappedSamples.Select(x => (object)new
            {
                type = "Feature", geometry = new { type = "Point", coordinates = new[] { x.Location.X, x.Location.Y } },
                properties = new { x.Id, x.SampleCode, x.Status, x.SampledAt, x.DepthFromCm, x.DepthToCm, layer = "soil-samples" }
            }).ToArray();
        return UseCaseResults.Ok(new
        {
            farm = new { farm.Id, farm.OrganisationNodeId, farm.Name, farm.Code, farm.CountryCode, farm.TimeZone },
            summary = new { fields, activeTenures, infrastructure, activeSoilZones, soilSamples, analysedSamples, latestSample },
            map = new { type = "FeatureCollection", features = assetLayer.Concat(sampleLayer) }
        });
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
