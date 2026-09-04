using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.AcceptInvitation;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
using SupplierInvitationStatus = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitationStatus;

public sealed class AcceptInvitationHandler(IPlatformDbContext db) : IRequestHandler<AcceptInvitationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AcceptInvitationCommand message, CancellationToken cancellationToken) => AcceptInvitationAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> AcceptInvitationAsync(AcceptSupplierInvitationRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        string hash = Hash(request.Token);
        SupplierInvitation? invitation = await db.SupplierInvitations.SingleOrDefaultAsync(x => x.TokenHash == hash, ct);
        if (invitation is null) return UseCaseResults.NotFound();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if (now >= invitation.ExpiresAt)
        {
            if (invitation.Status == SupplierInvitationStatus.Pending) { invitation.Expire(now); await db.SaveChangesAsync(ct); }
            return UseCaseResults.Problem(statusCode: 410, title: "Invitation expired", detail: "Request a new supplier invitation.");
        }
        try
        {
            var supplier = new HostedSupplierAccount(Guid.NewGuid(), request.SupplierCode, invitation.SupplierName,
                request.ContactName, invitation.Email, request.Phone);
            db.HostedSupplierAccounts.Add(supplier); invitation.Accept(supplier.Id, now); await db.SaveChangesAsync(ct);
            return UseCaseResults.Ok(new { invitation, supplier });
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Bad("invitation_acceptance_invalid", e.Message); }
    }

    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value?.Trim() ?? string.Empty))).ToLowerInvariant();

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
