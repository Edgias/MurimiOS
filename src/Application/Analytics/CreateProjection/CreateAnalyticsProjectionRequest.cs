using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateProjection;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
public sealed record CreateAnalyticsProjectionRequest(Guid? Id, DateOnly PeriodStart, DateOnly PeriodEnd, string Currency, string ProjectionVersion, DateTimeOffset? ScheduledFor);
