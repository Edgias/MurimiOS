using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateCohort;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record CreateBenchmarkCohortRequest(Guid? Id, string CohortKey, int DefinitionVersion, string Name,
    string Description, string InclusionCriteriaJson, int MinimumCohortSize, DateOnly EffectiveFrom,
    DateOnly? EffectiveTo, string Owner);
