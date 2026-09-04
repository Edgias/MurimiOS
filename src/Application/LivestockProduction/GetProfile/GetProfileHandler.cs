using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.GetProfile;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class GetProfileHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetProfileQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetProfileQuery message, CancellationToken cancellationToken) => GetProfileAsync(message.subjectId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetProfileAsync(Guid subjectId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await AccessibleSubject(subjectId, db, scope, cancellationToken);
        if (subject is null) return UseCaseResults.NotFound();
        LivestockBiologicalProfile? profile = await db.LivestockBiologicalProfiles.AsNoTracking()
            .SingleOrDefaultAsync(x => x.SubjectId == subjectId, cancellationToken);
        return profile is null ? UseCaseResults.NotFound() : UseCaseResults.Ok(ProfileResponse(profile));
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }


    private static object ProfileResponse(LivestockBiologicalProfile x) => new { x.Id, x.SubjectId, x.Sex, x.BornOn, x.Breed, x.DamId, x.SireId };

}
