using EdgIQ.MurimiOS.Application.Common.Livestock;
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
namespace EdgIQ.MurimiOS.Application.LivestockExchange.InstallDefaults;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

public sealed class InstallDefaultsHandler(IPlatformDbContext db) : IRequestHandler<InstallDefaultsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(InstallDefaultsCommand message, CancellationToken cancellationToken) => InstallDefaultsAsync(db, cancellationToken);

    private static async Task<UseCaseResult> InstallDefaultsAsync(IPlatformDbContext db, CancellationToken cancellationToken)
    {
        LivestockRegulatoryProfile[] profiles = await LivestockRegulatoryDefaults.EnsureDefaultsAsync(db, cancellationToken);
        return UseCaseResults.Ok(profiles.Select(ProfileResponse).ToArray());
    }


    private static object ProfileResponse(LivestockRegulatoryProfile x) => new { x.Id, x.ProfileKey, x.ProfileVersion,
        x.Name, x.ExchangeType, x.Jurisdiction, x.Species, x.ProviderCode, x.SchemaVersion, x.MediaType,
        x.ReportingDeadlineHours, x.RegisterDeadlineHours, x.EffectiveFrom, x.EffectiveTo, x.SourceUri, x.IsBuiltIn };

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
