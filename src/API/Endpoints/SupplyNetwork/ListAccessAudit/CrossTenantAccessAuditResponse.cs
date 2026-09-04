using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListAccessAudit;
public sealed record CrossTenantAccessAuditResponse(Guid Id, Guid ProducerTenantId, Guid RecipientTenantId,
    Guid GrantId, Guid ActorTenantId, string ActorSubject, string Purpose, string EntityCategory,
    Guid? ResourceId, DateTimeOffset OccurredAt, int ResultCount, string Outcome, string? CorrelationId);
