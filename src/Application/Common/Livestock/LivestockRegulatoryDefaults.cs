using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Livestock;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;


public static class LivestockRegulatoryDefaults
{
    public const string IcarAdeProvider = "icar-ade";

    public const string ScotlandCattleMovementProfileKey = "gb-sct-cattle-movement";

    public const string IcarCattleRegistrationProfileKey = "icar-ade-cattle-registration";

    public const string IcarCattleMilkProfileKey = "icar-ade-cattle-milk-recording";


    public static async Task<LivestockRegulatoryProfile[]> EnsureDefaultsAsync(IPlatformDbContext db,
        CancellationToken cancellationToken)
    {
        LivestockRegulatoryProfile[] defaults = DefaultProfiles();
        string[] keys = defaults.Select(x => x.ProfileKey).ToArray();
        var existing = await db.LivestockRegulatoryProfiles
            .Where(x => keys.Contains(x.ProfileKey) && x.ProfileVersion == 1)
            .Select(x => new { x.ProfileKey, x.ProfileVersion }).ToArrayAsync(cancellationToken);
        foreach (LivestockRegulatoryProfile profile in defaults.Where(candidate => !existing.Any(current =>
            current.ProfileKey == candidate.ProfileKey && current.ProfileVersion == candidate.ProfileVersion)))
            db.LivestockRegulatoryProfiles.Add(profile);
        if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync(cancellationToken);
        return await db.LivestockRegulatoryProfiles.AsNoTracking()
            .Where(x => keys.Contains(x.ProfileKey) && x.ProfileVersion == 1)
            .OrderBy(x => x.ExchangeType).ToArrayAsync(cancellationToken);
    }


    public static LivestockRegulatoryProfile[] DefaultProfiles() =>
    [
        new(Guid.NewGuid(), ScotlandCattleMovementProfileKey, 1, "Scotland cattle movement reporting",
            LivestockRegulatoryExchangeType.Movement, "GB-SCT", "cattle", "scoteid", "1.7.9",
            "application/xml", 72, 48, new DateOnly(2021, 10, 4), null,
            "https://www.gov.scot/publications/livestock-identification-and-traceability-guidance/pages/cattle/", true),
        new(Guid.NewGuid(), IcarCattleRegistrationProfileKey, 1, "ICAR ADE cattle registration",
            LivestockRegulatoryExchangeType.BreedRegistration, "GLOBAL", "cattle", IcarAdeProvider, "1.5.0",
            "application/json", null, null, new DateOnly(2025, 1, 1), null,
            "https://www.icar.org/group/working-group-animal-data-exchange/", true),
        new(Guid.NewGuid(), IcarCattleMilkProfileKey, 1, "ICAR ADE cattle milk recording",
            LivestockRegulatoryExchangeType.MilkRecording, "GLOBAL", "cattle", IcarAdeProvider, "1.5.0",
            "application/json", null, null, new DateOnly(2025, 1, 1), null,
            "https://www.icar.org/the-icar-ade-standards/", true)
    ];

}
