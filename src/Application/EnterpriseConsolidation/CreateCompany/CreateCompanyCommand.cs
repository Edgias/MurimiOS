using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateCompany;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record CreateCompanyCommand(CreateLegalCompanyRequest request) : IRequest<UseCaseResult>;
