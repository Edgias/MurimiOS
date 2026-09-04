using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ResumeSchedule;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;

public sealed record ResumeScheduleCommand(Guid scheduleId, ResumeAnalyticsScheduleRequest request) : IRequest<UseCaseResult>;
