using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.GetFarmsFarmIdGuidFields;


public sealed partial class GetFarmsFarmIdGuidFieldsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetFarmsFarmIdGuidFieldsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetFarmsFarmIdGuidFieldsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
            var results = await (
                from field in db.FarmFields
                join boundary in db.FieldBoundaryVersions on field.Id equals boundary.FieldId
                where field.FarmId == farmId && boundary.ValidTo == null
                orderby field.Name
                select new FieldResponse(
                    field.Id,
                    field.FarmId,
                    field.Name,
                    boundary.Id,
                    boundary.AreaHa,
                    boundary.ValidFrom))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}
