using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateSchedule;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;

public sealed record CreateScheduleCommand(Guid farmId, CreateAnalyticsScheduleRequest request) : IRequest<UseCaseResult>;
