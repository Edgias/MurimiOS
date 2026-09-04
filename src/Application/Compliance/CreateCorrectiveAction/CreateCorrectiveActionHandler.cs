using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.CreateCorrectiveAction;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;

public sealed class CreateCorrectiveActionHandler(IPlatformDbContext db) : IRequestHandler<CreateCorrectiveActionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCorrectiveActionCommand message, CancellationToken cancellationToken) => CreateCorrectiveActionAsync(message.findingId, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateCorrectiveActionAsync(Guid findingId, CreateCorrectiveActionRequest request,
        IPlatformDbContext db, CancellationToken ct)
    {
        ComplianceFinding? finding = await db.ComplianceFindings.SingleOrDefaultAsync(x => x.Id == findingId, ct);
        if (finding is null) return UseCaseResults.NotFound();
        try
        {
            var action = new ComplianceCorrectiveAction(Id(request.Id), findingId, request.Action, request.Owner, request.DueOn);
            db.ComplianceCorrectiveActions.Add(action);
            if (finding.Status == ComplianceFindingStatus.Open) finding.MarkActionPending();
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/compliance/findings/{findingId}/actions/{action.Id}", action);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Bad("corrective_action_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
