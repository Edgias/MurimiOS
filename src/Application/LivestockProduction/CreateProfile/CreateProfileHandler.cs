using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.CreateProfile;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockSex = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSex;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed class CreateProfileHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateProfileCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateProfileCommand message, CancellationToken cancellationToken) => CreateProfileAsync(message.subjectId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateProfileAsync(Guid subjectId, CreateBiologicalProfileRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await AccessibleSubject(subjectId, db, scope, cancellationToken);
        if (subject is null) return UseCaseResults.NotFound();
        if (subject.Kind != LivestockSubjectKind.Individual) return Bad("individual_required", "Biological profiles belong to individual animals.");
        if (await db.LivestockBiologicalProfiles.AnyAsync(x => x.SubjectId == subjectId, cancellationToken))
            return UseCaseResults.Conflict(new { code = "biological_profile_exists", message = "Use the correction workflow to replace biological data." });
        Guid[] parentIds = new[] { request.DamId, request.SireId }.Where(x => x.HasValue).Select(x => x!.Value).ToArray();
        if (parentIds.Length > 0 && await db.LivestockSubjects.CountAsync(x => parentIds.Contains(x.Id) && x.FarmId == subject.FarmId
                && x.Kind == LivestockSubjectKind.Individual && x.Species == subject.Species, cancellationToken) != parentIds.Length)
            return Bad("parent_scope_invalid", "Parents must be individual animals of the same species and farm.");
        if (request.DamId is { } damId && await db.LivestockBiologicalProfiles.Where(x => x.SubjectId == damId)
                .Select(x => (LivestockSex?)x.Sex).SingleOrDefaultAsync(cancellationToken) != LivestockSex.Female)
            return Bad("dam_profile_invalid", "A recorded dam must have a female biological profile.");
        if (request.SireId is { } sireId && await db.LivestockBiologicalProfiles.Where(x => x.SubjectId == sireId)
                .Select(x => (LivestockSex?)x.Sex).SingleOrDefaultAsync(cancellationToken) != LivestockSex.Male)
            return Bad("sire_profile_invalid", "A recorded sire must have a male biological profile.");
        try
        {
            var profile = new LivestockBiologicalProfile(Guid.NewGuid(), subjectId, request.Sex, request.BornOn,
                request.Breed, request.DamId, request.SireId);
            db.LivestockBiologicalProfiles.Add(profile);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/livestock/subjects/{subjectId}/biological-profile", ProfileResponse(profile));
        }
        catch (ArgumentException exception) { return Bad("biological_profile_invalid", exception.Message); }
    }


    private static async Task<LivestockSubject?> AccessibleSubject(Guid id, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        LivestockSubject? subject = await db.LivestockSubjects.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return subject is not null && await scope.CanAccessFarmAsync(subject.FarmId, cancellationToken) ? subject : null;
    }


    private static object ProfileResponse(LivestockBiologicalProfile x) => new { x.Id, x.SubjectId, x.Sex, x.BornOn, x.Breed, x.DamId, x.SireId };

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
