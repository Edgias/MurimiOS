using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Compliance.WaiveFinding;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;

public sealed class WaiveFindingHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<WaiveFindingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(WaiveFindingCommand message, CancellationToken cancellationToken) => WaiveFindingAsync(message.findingId, message.request, db, actor, cancellationToken);

    private static async Task<UseCaseResult> WaiveFindingAsync(Guid findingId, CorrectiveActionCommandRequest request,
        IPlatformDbContext db, IRequestActorContext actor, CancellationToken ct)
    {
        ComplianceFinding? finding = await db.ComplianceFindings.SingleOrDefaultAsync(x => x.Id == findingId, ct);
        if (finding is null) return UseCaseResults.NotFound();
        try { finding.Waive(DateTimeOffset.UtcNow, actor.Subject, request.Notes ?? "Waived"); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(finding); }
        catch (InvalidOperationException e) { return Conflict("finding_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
