using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ChangeKpi;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class ChangeKpiHandler(IPlatformDbContext db) : IRequestHandler<ChangeKpiCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeKpiCommand message, CancellationToken cancellationToken) => ChangeKpiAsync(message.kpiId, message.command, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeKpiAsync(Guid kpiId, string command, IPlatformDbContext db, CancellationToken ct)
    { KpiDefinition? kpi = await db.KpiDefinitions.SingleOrDefaultAsync(x => x.Id == kpiId, ct); if (kpi is null) return UseCaseResults.NotFound(); try { if (command == "activate") kpi.Activate(DateTimeOffset.UtcNow); else kpi.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(kpi); } catch (InvalidOperationException e) { return Conflict("kpi_transition_invalid", e.Message); } }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
