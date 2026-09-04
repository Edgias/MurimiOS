using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.Shared;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;
public sealed record SupplyRelationshipResponse(Guid Id, Guid ProducerTenantId, Guid BuyerTenantId,
    Guid RequestedByTenantId, string Name, SupplyRelationshipStatus Status, DateTimeOffset RequestedAt,
    DateTimeOffset? RespondedAt, DateTimeOffset? RevokedAt, Guid? RevokedByTenantId, string? RevocationReason, uint Version);
