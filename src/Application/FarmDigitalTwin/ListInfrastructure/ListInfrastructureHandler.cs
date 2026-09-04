using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListInfrastructure;
using InfrastructureAssetStatus = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.InfrastructureAssetStatus;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;

public sealed class ListInfrastructureHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListInfrastructureQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListInfrastructureQuery message, CancellationToken cancellationToken) => ListInfrastructureAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListInfrastructureAsync(Guid farmId, InfrastructureAssetStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<SiteInfrastructureAsset> query = db.SiteInfrastructureAssets.AsNoTracking().Where(x => x.FarmId == farmId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        SiteInfrastructureAsset[] rows = await query.OrderBy(x => x.Code).ToArrayAsync(ct);
        return UseCaseResults.Ok(rows.Select(Asset));
    }


    private static object Asset(SiteInfrastructureAsset x) => new
    {
        x.Id, x.FarmId, x.FieldId, x.Code, x.Name, x.Type, x.Status,
        location = x.Location is null ? null : new { longitude = x.Location.X, latitude = x.Location.Y },
        x.InstalledOn, x.Capacity, x.CapacityUnit, details = ParseJson(x.DetailsJson),
        x.DecommissionedOn, x.DecommissionedAt, x.DecommissionedBy, x.DecommissionReason
    };

    private static JsonElement ParseJson(string json) { using JsonDocument document = JsonDocument.Parse(json); return document.RootElement.Clone(); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
