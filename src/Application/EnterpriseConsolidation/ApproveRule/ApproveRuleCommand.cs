using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ApproveRule;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record ApproveRuleCommand(Guid id, ApproveEliminationRuleRequest request) : IRequest<UseCaseResult>;
