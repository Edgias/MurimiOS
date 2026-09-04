using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.GetConsolidation;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;

public sealed record GetConsolidationQuery(Guid id) : IRequest<UseCaseResult>;
