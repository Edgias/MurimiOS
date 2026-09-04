using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ChangeClient;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class ChangeClientHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context) : IRequestHandler<ChangeClientCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeClientCommand message, CancellationToken cancellationToken) => ChangeClientAsync(message.clientId, message.command, db, actor, tenant, context, cancellationToken);

    private static async Task<UseCaseResult> ChangeClientAsync(Guid clientId, string command, IPlatformDbContext db, IRequestActorContext actor,
        ITenantContext tenant, UseCaseContext context, CancellationToken ct)
    {
        PublicApiClient? client = await db.PublicApiClients.SingleOrDefaultAsync(x => x.Id == clientId, ct); if (client is null) return UseCaseResults.NotFound();
        try { if (command == "activate") client.Activate(); else if (command == "suspend") client.Suspend(); else client.Revoke(DateTimeOffset.UtcNow, actor.Subject); AddManagementRecords(db, tenant, actor, command, client, context, DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(client); }
        catch (InvalidOperationException e) { return Conflict("api_client_transition_invalid", e.Message); }
    }

    private static void AddManagementRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, string action, PublicApiClient client, UseCaseContext context, DateTimeOffset now) { string json = JsonSerializer.Serialize(new { client.Id, client.Name, client.Scopes, client.FarmIds, client.Status }); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, nameof(PublicApiClient), client.Id, null, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"PublicApi.Client{action}", json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

}
