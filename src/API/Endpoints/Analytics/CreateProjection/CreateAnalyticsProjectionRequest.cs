
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateProjection;
public sealed record CreateAnalyticsProjectionRequest(Guid? Id, DateOnly PeriodStart, DateOnly PeriodEnd, string Currency, string ProjectionVersion, DateTimeOffset? ScheduledFor);
