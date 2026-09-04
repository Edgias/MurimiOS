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
namespace EdgIQ.MurimiOS.Application.Livestock.PostLivestockSubjects;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed partial class PostLivestockSubjectsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostLivestockSubjectsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostLivestockSubjectsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            CreateLivestockSubjectRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(request.FarmId, cancellationToken)
                || !await db.Farms.AnyAsync(x => x.Id == request.FarmId, cancellationToken))
            {
                return UseCaseResults.BadRequest(new { code = "farm_not_found", message = "Farm not found in this tenant." });
            }

            Guid id = request.Id is { } requestedId && requestedId != Guid.Empty ? requestedId : Guid.NewGuid();
            var subject = new LivestockSubject(
                id,
                request.FarmId,
                request.Kind,
                request.Name,
                request.Species,
                request.OfficialIdentifier);
            db.LivestockSubjects.Add(subject);
            await db.SaveChangesAsync(cancellationToken);

            return UseCaseResults.Created(
                $"/api/v1/livestock/subjects/{id}",
                new LivestockSubjectResponse(
                    id,
                    subject.FarmId,
                    subject.Kind,
                    subject.Name,
                    subject.Species,
                    subject.OfficialIdentifier));
        }

}
