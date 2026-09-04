
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.ResumeSchedule;
public sealed record ResumeAnalyticsScheduleRequest(DateTimeOffset NextRunAt);
