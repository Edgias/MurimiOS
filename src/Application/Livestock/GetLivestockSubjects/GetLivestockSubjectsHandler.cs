using EdgIQ.MurimiOS.Domain.LivestockSubject;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Livestock.GetLivestockSubjects;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed partial class GetLivestockSubjectsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetLivestockSubjectsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetLivestockSubjectsQuery message, CancellationToken cancellationToken) => ExecuteAsync(db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, CancellationToken cancellationToken)         {
            ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
            IQueryable<LivestockSubject> query = db.LivestockSubjects;
            if (!scope.IsUnrestricted) query = query.Where(subject => scope.FarmIds!.Contains(subject.FarmId));
            LivestockSubjectResponse[] results = await query
                .OrderBy(x => x.Name)
                .Select(x => new LivestockSubjectResponse(
                    x.Id,
                    x.FarmId,
                    x.Kind,
                    x.Name,
                    x.Species,
                    x.OfficialIdentifier))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}
