using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.DecideApproval;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using ApprovalDecision = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecision;
using ApprovalDecisionKind = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecisionKind;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed record DecideApprovalCommand(Guid requestId, ApprovalDecisionRequest request) : IRequest<UseCaseResult>;
