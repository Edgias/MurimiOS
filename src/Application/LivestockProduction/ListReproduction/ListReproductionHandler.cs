using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ListReproduction;
using LivestockOffspringLink = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockOffspringLink;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class ListReproductionHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListReproductionQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListReproductionQuery message, CancellationToken cancellationToken) => ListReproductionAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListReproductionAsync(Guid subjectId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (await AccessibleSubject(subjectId, db, scope, cancellationToken) is null) return UseCaseResults.NotFound();
        LivestockReproductionEvent[] events = await db.LivestockReproductionEvents.AsNoTracking()
            .Where(x => x.SubjectId == subjectId).OrderByDescending(x => x.OccurredAt).ToArrayAsync(cancellationToken);
        Guid[] eventIds = events.Select(x => x.Id).ToArray();
        LivestockOffspringLink[] links = await db.LivestockOffspringLinks.AsNoTracking()
            .Where(x => eventIds.Contains(x.ParturitionEventId)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(events.Select(x => ReproductionResponse(x, links.Where(link => link.ParturitionEventId == x.Id))).ToArray());
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }

    private static object ReproductionResponse(LivestockReproductionEvent x, IEnumerable<LivestockOffspringLink> links) => new
    {
        x.Id, x.FarmId, x.SubjectId, x.Type, x.Outcome, x.OccurredAt, x.RelatedSubjectId, x.BreedingEventId,
        x.Method, x.ExpectedDueOn, x.OffspringCount, x.WeightKg, x.EnteredWeight, x.EnteredWeightUnit,
        x.Practitioner, x.Notes, Offspring = links.Select(link => new { link.Id, link.OffspringSubjectId, link.Outcome,
            link.BirthWeightKg, link.EnteredBirthWeight, link.EnteredBirthWeightUnit }).ToArray()
    };

}
