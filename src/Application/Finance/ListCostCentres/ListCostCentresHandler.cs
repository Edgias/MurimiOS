using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CostCentre;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ListCostCentres;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;

public sealed class ListCostCentresHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListCostCentresQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCostCentresQuery message, CancellationToken cancellationToken) => ListCostCentresAsync(message.farmId, message.active, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListCostCentresAsync(Guid farmId, bool? active, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        IQueryable<CostCentre> query = db.CostCentres.AsNoTracking().Where(x => x.FarmId == farmId);
        if (active is { } isActive) query = query.Where(x => x.IsActive == isActive);
        CostCentre[] rows = await query.OrderBy(x => x.Code).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToCostCentre).ToArray());
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


    private static CostCentreResponse ToCostCentre(CostCentre x) => new(x.Id, x.FarmId, x.Code, x.Name,
        x.Type, x.TargetId, x.Currency, x.IsActive, x.ClosedAt, x.ClosedBy);

}
