
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.EnrollBenchmark;
public sealed record CreateBenchmarkEnrollmentRequest(Guid? Id, string MetricCode, Guid? CohortDefinitionId);
