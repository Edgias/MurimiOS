using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Audit.List;

public sealed record AuditEventResponse(
    Guid Id,
    DateTimeOffset OccurredAt,
    string ActorSubject,
    Guid? MembershipId,
    string Action,
    string EntityType,
    Guid EntityId,
    Guid? FarmId,
    Guid? OrganisationNodeId,
    JsonElement Changes,
    string? CorrelationId);
