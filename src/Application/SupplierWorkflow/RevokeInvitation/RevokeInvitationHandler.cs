using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.RevokeInvitation;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class RevokeInvitationHandler(IPlatformDbContext db) : IRequestHandler<RevokeInvitationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeInvitationCommand message, CancellationToken cancellationToken) => RevokeInvitationAsync(message.invitationId, db, cancellationToken);

    private static async Task<UseCaseResult> RevokeInvitationAsync(Guid invitationId, IPlatformDbContext db, CancellationToken ct)
    {
        SupplierInvitation? invitation = await db.SupplierInvitations.SingleOrDefaultAsync(x => x.Id == invitationId, ct);
        if (invitation is null) return UseCaseResults.NotFound();
        try { invitation.Revoke(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(invitation); }
        catch (InvalidOperationException e) { return Conflict("invitation_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
