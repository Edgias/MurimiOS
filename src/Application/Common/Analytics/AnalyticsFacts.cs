using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Analytics;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;


public static class AnalyticsFacts
{

    public static IQueryable<AnalyticsFact> CurrentFacts(IPlatformDbContext db) => db.AnalyticsFacts.AsNoTracking()
        .Where(x => db.AnalyticsProjectionRuns.Any(run => run.Id == x.ProjectionRunId
            && run.Status == AnalyticsProjectionStatus.Succeeded && run.SupersededAt == null));

}
