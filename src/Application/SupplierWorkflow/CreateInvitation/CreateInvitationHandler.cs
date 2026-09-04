using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateInvitation;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class CreateInvitationHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<CreateInvitationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateInvitationCommand message, CancellationToken cancellationToken) => CreateInvitationAsync(message.request, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> CreateInvitationAsync(CreateSupplierInvitationRequest request, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        if (request.ExpiresAt <= DateTimeOffset.UtcNow) return Bad("expiry_invalid", "Invitation expiry must be in the future.");
        string rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        try
        {
            var invitation = new SupplierInvitation(Id(request.Id), request.SupplierName, request.Email, Hash(rawToken),
                request.ExpiresAt, actor.Subject, DateTimeOffset.UtcNow);
            db.SupplierInvitations.Add(invitation); AddRecords(db, tenant, actor, "Invited", invitation.Id, invitation, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/invitations/{invitation.Id}", new { invitation, token = rawToken });
        }
        catch (ArgumentException e) { return Bad("invitation_invalid", e.Message); }
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, Guid entityId, object payload, UseCaseContext context)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, "SupplierWorkflow", entityId, null, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"Supplier{action}", json, now));
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value?.Trim() ?? string.Empty))).ToLowerInvariant();

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
