using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.Download;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

public sealed class DownloadHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IMediaStorage storage) : IRequestHandler<DownloadQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DownloadQuery message, CancellationToken cancellationToken) => DownloadAsync(message.evidenceId, db, scopeAccess, storage, cancellationToken);

    private static async Task<UseCaseResult> DownloadAsync(
        Guid evidenceId,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        IMediaStorage storage,
        CancellationToken cancellationToken)
    {
        EvidenceAttachment? evidence = await db.EvidenceAttachments
            .SingleOrDefaultAsync(x => x.Id == evidenceId, cancellationToken);
        if (evidence is null)
        {
            return UseCaseResults.NotFound();
        }
        Guid? farmId = await FarmIdForCropZoneAsync(evidence.CropZoneId, db, cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
            return UseCaseResults.NotFound();

        try
        {
            Stream stream = await storage.OpenReadAsync(evidence.ObjectKey, cancellationToken);
            return UseCaseResults.File(
                stream,
                evidence.ContentType,
                evidence.FileName,
                enableRangeProcessing: true);
        }
        catch (FileNotFoundException)
        {
            return UseCaseResults.NotFound();
        }
    }


    private static Task<Guid?> FarmIdForCropZoneAsync(
        Guid cropZoneId,
        IPlatformDbContext db,
        CancellationToken cancellationToken) =>
        (from zone in db.CropZones
         join season in db.GrowingSeasons on zone.SeasonId equals season.Id
         where zone.Id == cropZoneId
         select (Guid?)season.FarmId).SingleOrDefaultAsync(cancellationToken);

}
