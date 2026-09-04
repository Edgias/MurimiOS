using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
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
public sealed record SupplyRelationshipResponse(Guid Id, Guid ProducerTenantId, Guid BuyerTenantId,
    Guid RequestedByTenantId, string Name, SupplyRelationshipStatus Status, DateTimeOffset RequestedAt,
    DateTimeOffset? RespondedAt, DateTimeOffset? RevokedAt, Guid? RevokedByTenantId, string? RevocationReason, uint Version);
