using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateSchedule;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;
public sealed record CreateAnalyticsScheduleRequest(Guid? Id, string Name, AnalyticsScheduleFrequency Frequency, int LookbackDays, string Currency, string ProjectionVersion, DateTimeOffset NextRunAt);
