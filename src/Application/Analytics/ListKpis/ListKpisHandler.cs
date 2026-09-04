using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ListKpis;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;
using KpiDefinitionStatus = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinitionStatus;

public sealed class ListKpisHandler(IPlatformDbContext db) : IRequestHandler<ListKpisQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListKpisQuery message, CancellationToken cancellationToken) => ListKpisAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListKpisAsync(KpiDefinitionStatus? status, IPlatformDbContext db, CancellationToken ct)
    { IQueryable<KpiDefinition> query = db.KpiDefinitions.AsNoTracking(); if (status.HasValue) query = query.Where(x => x.Status == status); return UseCaseResults.Ok(await query.OrderBy(x => x.Code).ThenByDescending(x => x.DefinitionVersion).ToArrayAsync(ct)); }


}
