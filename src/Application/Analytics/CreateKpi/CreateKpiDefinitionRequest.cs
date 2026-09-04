using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateKpi;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed record CreateKpiDefinitionRequest(Guid? Id, Guid? SeriesId, string Code, string Name, string Description, string Formula, string Unit, string TimeGrain, string Owner, string Exclusions, string QualityRule, int Version);
