using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CropComparison;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsFactKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFactKind;

public sealed record CropComparisonQuery(string metricCode, DateOnly from, DateOnly to, Guid? farmId) : IRequest<UseCaseResult>;
