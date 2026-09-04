using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.ListAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;

using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;
public sealed record CrossTenantAccessAuditResponse(Guid Id, Guid ProducerTenantId, Guid RecipientTenantId,
    Guid GrantId, Guid ActorTenantId, string ActorSubject, string Purpose, string EntityCategory,
    Guid? ResourceId, DateTimeOffset OccurredAt, int ResultCount, string Outcome, string? CorrelationId);
