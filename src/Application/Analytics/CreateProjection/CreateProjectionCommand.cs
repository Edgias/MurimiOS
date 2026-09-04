using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateProjection;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

public sealed record CreateProjectionCommand(Guid farmId, CreateAnalyticsProjectionRequest request) : IRequest<UseCaseResult>;
