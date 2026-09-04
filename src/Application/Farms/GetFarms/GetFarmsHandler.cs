using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.GetFarms;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

public sealed partial class GetFarmsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetFarmsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetFarmsQuery message, CancellationToken cancellationToken) => ExecuteAsync(db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, CancellationToken cancellationToken)         {
            ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
            IQueryable<Farm> query = db.Farms;
            if (!scope.IsUnrestricted) query = query.Where(farm => scope.FarmIds!.Contains(farm.Id));
            FarmResponse[] results = await query
                .OrderBy(x => x.Name)
                .Select(x => new FarmResponse(x.Id, x.OrganisationNodeId, x.Name, x.Code, x.CountryCode, x.TimeZone))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}
