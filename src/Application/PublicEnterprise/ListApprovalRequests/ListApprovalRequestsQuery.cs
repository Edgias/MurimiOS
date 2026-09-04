using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ListApprovalRequests;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using DelegatedApprovalStatus = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalStatus;

public sealed record ListApprovalRequestsQuery(DelegatedApprovalStatus? status) : IRequest<UseCaseResult>;
