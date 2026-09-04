using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ListCompanies;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;

public sealed class ListCompaniesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListCompaniesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCompaniesQuery message, CancellationToken cancellationToken) => ListCompaniesAsync(message.active, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListCompaniesAsync(bool? active, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        IQueryable<LegalCompany> query = db.LegalCompanies.AsNoTracking();
        if (!resolved.IsUnrestricted) query = query.Where(x => resolved.OrganisationNodeIds!.Contains(x.OrganisationNodeId));
        if (active is { } isActive) query = query.Where(x => x.IsActive == isActive);
        return UseCaseResults.Ok(await query.OrderBy(x => x.CompanyCode).ToArrayAsync(ct));
    }


}
