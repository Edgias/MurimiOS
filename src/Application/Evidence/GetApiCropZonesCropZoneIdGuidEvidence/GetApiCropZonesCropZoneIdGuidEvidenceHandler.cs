using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.GetApiCropZonesCropZoneIdGuidEvidence;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

public sealed class GetApiCropZonesCropZoneIdGuidEvidenceHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiCropZonesCropZoneIdGuidEvidenceQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiCropZonesCropZoneIdGuidEvidenceQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.cropZoneId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid cropZoneId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            Guid? farmId = await FarmIdForCropZoneAsync(cropZoneId, db, cancellationToken);
            if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
                return UseCaseResults.NotFound();
            EvidenceAttachment[] evidence = await db.EvidenceAttachments
                .Where(x => x.CropZoneId == cropZoneId)
                .OrderByDescending(x => x.CapturedAt)
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(evidence.Select(ToResponse).ToArray());
        }

    private static EvidenceResponse ToResponse(EvidenceAttachment evidence) => new(
        evidence.Id,
        evidence.CropZoneId,
        evidence.FileName,
        evidence.ContentType,
        evidence.ByteSize,
        evidence.Sha256,
        evidence.CapturedAt,
        evidence.Latitude,
        evidence.Longitude,
        evidence.AccuracyMetres,
        $"/api/v1/evidence/{evidence.Id}/content");


    private static Task<Guid?> FarmIdForCropZoneAsync(
        Guid cropZoneId,
        IPlatformDbContext db,
        CancellationToken cancellationToken) =>
        (from zone in db.CropZones
         join season in db.GrowingSeasons on zone.SeasonId equals season.Id
         where zone.Id == cropZoneId
         select (Guid?)season.FarmId).SingleOrDefaultAsync(cancellationToken);

}
