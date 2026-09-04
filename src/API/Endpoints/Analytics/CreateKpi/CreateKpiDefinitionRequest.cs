
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateKpi;

public sealed record CreateKpiDefinitionRequest(Guid? Id, Guid? SeriesId, string Code, string Name, string Description, string Formula, string Unit, string TimeGrain, string Owner, string Exclusions, string QualityRule, int Version);
