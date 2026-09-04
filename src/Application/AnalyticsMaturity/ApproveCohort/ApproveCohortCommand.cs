using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ApproveCohort;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record ApproveCohortCommand(Guid id, AnalyticsApprovalRequest request) : IRequest<UseCaseResult>;
