using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateSchedule;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;
public sealed record CreateAnalyticsScheduleRequest(Guid? Id, string Name, AnalyticsScheduleFrequency Frequency, int LookbackDays, string Currency, string ProjectionVersion, DateTimeOffset NextRunAt);
