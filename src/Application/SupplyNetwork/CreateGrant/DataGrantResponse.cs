using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.CreateGrant;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;
public sealed record DataGrantResponse(Guid Id, Guid GrantSeriesId, Guid RelationshipId, Guid ProducerTenantId,
    Guid RecipientTenantId, string Name, string Purpose, IReadOnlyList<string> EntityCategories,
    IReadOnlyList<string> Attributes, IReadOnlyList<Guid> FarmIds, IReadOnlyList<Guid> SeasonIds,
    IReadOnlyList<string> Commodities, DateTimeOffset ValidFrom, DateTimeOffset? ValidTo,
    bool DownstreamDisclosureAllowed, int GrantVersion, Guid? PreviousGrantId, DataGrantStatus Status,
    DateTimeOffset? RevokedAt, string? RevocationReason);
