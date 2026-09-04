using EdgIQ.MurimiOS.Domain.KpiDefinition;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ListKpis;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;
using KpiDefinitionStatus = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinitionStatus;

public sealed record ListKpisQuery(KpiDefinitionStatus? status) : IRequest<UseCaseResult>;
