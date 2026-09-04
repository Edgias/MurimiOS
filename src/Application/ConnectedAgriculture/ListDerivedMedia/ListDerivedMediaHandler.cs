using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDerivedMedia;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using DerivedMediaLifecycleStatus = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaLifecycleStatus;

public sealed class ListDerivedMediaHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListDerivedMediaQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDerivedMediaQuery message, CancellationToken cancellationToken) => ListDerivedMediaAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListDerivedMediaAsync(Guid farmId, DerivedMediaLifecycleStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<DerivedMediaAsset> query = db.DerivedMediaAssets.AsNoTracking().Where(x => x.FarmId == farmId);
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.GeneratedAt).ToArrayAsync(ct));
    }


}
