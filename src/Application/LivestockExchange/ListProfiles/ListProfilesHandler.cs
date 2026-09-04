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
namespace EdgIQ.MurimiOS.Application.LivestockExchange.ListProfiles;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;

public sealed class ListProfilesHandler(IPlatformDbContext db) : IRequestHandler<ListProfilesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListProfilesQuery message, CancellationToken cancellationToken) => ListProfilesAsync(message.exchangeType, message.jurisdiction, db, cancellationToken);

    private static async Task<UseCaseResult> ListProfilesAsync(LivestockRegulatoryExchangeType? exchangeType,
        string? jurisdiction, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        IQueryable<LivestockRegulatoryProfile> query = db.LivestockRegulatoryProfiles.AsNoTracking();
        if (exchangeType is { } type) query = query.Where(x => x.ExchangeType == type);
        if (!string.IsNullOrWhiteSpace(jurisdiction))
        {
            string normalized = jurisdiction.Trim().ToUpperInvariant();
            query = query.Where(x => x.Jurisdiction == normalized);
        }
        LivestockRegulatoryProfile[] rows = await query.OrderBy(x => x.ProfileKey).ThenByDescending(x => x.ProfileVersion)
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ProfileResponse).ToArray());
    }


    private static object ProfileResponse(LivestockRegulatoryProfile x) => new { x.Id, x.ProfileKey, x.ProfileVersion,
        x.Name, x.ExchangeType, x.Jurisdiction, x.Species, x.ProviderCode, x.SchemaVersion, x.MediaType,
        x.ReportingDeadlineHours, x.RegisterDeadlineHours, x.EffectiveFrom, x.EffectiveTo, x.SourceUri, x.IsBuiltIn };

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

}
