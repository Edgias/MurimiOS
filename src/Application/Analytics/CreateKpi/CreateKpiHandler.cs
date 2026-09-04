using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateKpi;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class CreateKpiHandler(IPlatformDbContext db) : IRequestHandler<CreateKpiCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateKpiCommand message, CancellationToken cancellationToken) => CreateKpiAsync(message.request, db, cancellationToken);

    private static async Task<UseCaseResult> CreateKpiAsync(CreateKpiDefinitionRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        try { var kpi = new KpiDefinition(Id(request.Id), Id(request.SeriesId), request.Code, request.Name, request.Description, request.Formula, request.Unit, request.TimeGrain, request.Owner, request.Exclusions, request.QualityRule, request.Version); db.KpiDefinitions.Add(kpi); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/analytics/kpis/{kpi.Id}", kpi); }
        catch (ArgumentException e) { return Bad("kpi_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
