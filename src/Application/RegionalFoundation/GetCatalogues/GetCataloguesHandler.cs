using System.Text.Json;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetCatalogues;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;

public sealed class GetCataloguesHandler(IPlatformDbContext db) : IRequestHandler<GetCataloguesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetCataloguesQuery message, CancellationToken cancellationToken) => GetCataloguesAsync(message.effectiveOn, db, cancellationToken);

    private static async Task<UseCaseResult> GetCataloguesAsync(
        DateOnly? effectiveOn,
        IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        IQueryable<ReferenceCatalogue> query = db.ReferenceCatalogues.AsNoTracking();
        if (effectiveOn is { } date)
            query = query.Where(x => x.EffectiveFrom <= date && (x.EffectiveTo == null || x.EffectiveTo >= date));
        ReferenceCatalogueResponse[] catalogues = await query
            .OrderBy(x => x.Key)
            .ThenByDescending(x => x.EffectiveFrom)
            .Select(x => new ReferenceCatalogueResponse(
                x.Id, x.Key, x.Name, x.CatalogueVersion, x.CountryCode, x.Authority,
                x.EffectiveFrom, x.EffectiveTo, x.Version))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(catalogues);
    }


}
