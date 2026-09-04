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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalPolicy;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed record CreateApprovalPolicyCommand(CreateApprovalPolicyRequest request) : IRequest<UseCaseResult>;
