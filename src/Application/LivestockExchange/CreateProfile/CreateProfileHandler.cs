using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockExchange.CreateProfile;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

public sealed class CreateProfileHandler(IPlatformDbContext db) : IRequestHandler<CreateProfileCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateProfileCommand message, CancellationToken cancellationToken) => CreateProfileAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateProfileAsync(CreateLivestockRegulatoryProfileRequest request,
        IPlatformDbContext db, CancellationToken cancellationToken)
    {
        Guid id = Id(request.Id);
        try
        {
            var profile = new LivestockRegulatoryProfile(id, request.ProfileKey, request.Version, request.Name,
                request.ExchangeType, request.Jurisdiction, request.Species, request.ProviderCode,
                request.SchemaVersion, request.MediaType, request.ReportingDeadlineHours,
                request.RegisterDeadlineHours, request.EffectiveFrom, request.EffectiveTo, request.SourceUri, false);
            if (await db.LivestockRegulatoryProfiles.AnyAsync(x => x.ProfileKey == profile.ProfileKey
                && x.ProfileVersion == profile.ProfileVersion, cancellationToken))
                return Conflict("livestock_regulatory_profile_version_exists", "This regulatory profile version already exists.");
            db.LivestockRegulatoryProfiles.Add(profile);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/livestock/regulatory-profiles/{id}", ProfileResponse(profile));
        }
        catch (ArgumentException ex) { return Bad("livestock_regulatory_profile_invalid", ex.Message); }
    }


    private static object ProfileResponse(LivestockRegulatoryProfile x) => new { x.Id, x.ProfileKey, x.ProfileVersion,
        x.Name, x.ExchangeType, x.Jurisdiction, x.Species, x.ProviderCode, x.SchemaVersion, x.MediaType,
        x.ReportingDeadlineHours, x.RegisterDeadlineHours, x.EffectiveFrom, x.EffectiveTo, x.SourceUri, x.IsBuiltIn };

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
