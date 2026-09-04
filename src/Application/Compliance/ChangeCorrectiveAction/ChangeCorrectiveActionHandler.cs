using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.ChangeCorrectiveAction;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed class ChangeCorrectiveActionHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<ChangeCorrectiveActionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeCorrectiveActionCommand message, CancellationToken cancellationToken) => ChangeCorrectiveActionAsync(message.findingId, message.actionId, message.command, message.request, db, actor, cancellationToken);

    private static async Task<UseCaseResult> ChangeCorrectiveActionAsync(Guid findingId, Guid actionId, string command,
        CorrectiveActionCommandRequest request, IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    {
        ComplianceFinding? finding = await db.ComplianceFindings.SingleOrDefaultAsync(x => x.Id == findingId, ct);
        ComplianceCorrectiveAction? action = await db.ComplianceCorrectiveActions.SingleOrDefaultAsync(x => x.Id == actionId && x.FindingId == findingId, ct);
        if (finding is null || action is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "start") action.Start();
            else if (command == "submit") { action.Submit(now, request.Notes ?? "Completed"); finding.MarkVerificationPending(); }
            else if (command == "verify")
            {
                if (!actor.HasPermission(PlatformPermissions.ComplianceVerify)) return UseCaseResults.Forbid();
                action.Verify(now, actor.Subject); finding.Close(now, actor.Subject);
            }
            else if (command == "reject") { if (!actor.HasPermission(PlatformPermissions.ComplianceVerify)) return UseCaseResults.Forbid(); action.Reject(request.Notes ?? "Rejected"); finding.Reopen(); }
            else action.Cancel();
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { finding, action });
        }
        catch (InvalidOperationException e) { return Conflict("corrective_action_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
