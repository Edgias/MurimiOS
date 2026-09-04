using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.Shared;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
public sealed record DataGrantResponse(Guid Id, Guid GrantSeriesId, Guid RelationshipId, Guid ProducerTenantId,
    Guid RecipientTenantId, string Name, string Purpose, IReadOnlyList<string> EntityCategories,
    IReadOnlyList<string> Attributes, IReadOnlyList<Guid> FarmIds, IReadOnlyList<Guid> SeasonIds,
    IReadOnlyList<string> Commodities, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo,
    bool DownstreamDisclosureAllowed, int GrantVersion, Guid? PreviousGrantId, DataGrantStatus Status,
    DateTimeOffset? RevokedAt, string? RevocationReason);
