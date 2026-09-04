using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ApproveModel;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;

using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
public sealed record AnalyticsApprovalRequest(string ApprovalReference);
