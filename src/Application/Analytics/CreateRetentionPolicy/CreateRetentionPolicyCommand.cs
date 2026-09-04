using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateRetentionPolicy;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;

public sealed record CreateRetentionPolicyCommand(CreateAnalyticsRetentionPolicyRequest request) : IRequest<UseCaseResult>;
