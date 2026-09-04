using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiFarmsFarmIdGuidInputs;


public sealed partial class GetApiFarmsFarmIdGuidInputsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiFarmsFarmIdGuidInputsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiFarmsFarmIdGuidInputsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
            AgriculturalInputResponse[] results = await db.AgriculturalInputs
                .Where(x => x.FarmId == farmId)
                .OrderBy(x => x.Name)
                .Select(x => new AgriculturalInputResponse(x.Id, x.FarmId, x.Name, x.Category, x.Unit, x.DisplayUnit))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}
