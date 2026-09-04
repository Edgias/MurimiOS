using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ChangeKpi;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed record ChangeKpiCommand(Guid kpiId, string command) : IRequest<UseCaseResult>;
