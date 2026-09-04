using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateCohort;

public sealed record CreateBenchmarkCohortRequest(Guid? Id, string CohortKey, int DefinitionVersion, string Name,
    string Description, string InclusionCriteriaJson, int MinimumCohortSize, DateOnly EffectiveFrom,
    DateOnly? EffectiveTo, string Owner);
