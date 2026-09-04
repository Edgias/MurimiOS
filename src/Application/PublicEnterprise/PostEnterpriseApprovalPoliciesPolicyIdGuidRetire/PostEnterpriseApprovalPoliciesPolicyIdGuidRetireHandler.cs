using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PostEnterpriseApprovalPoliciesPolicyIdGuidRetire;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;

public sealed class PostEnterpriseApprovalPoliciesPolicyIdGuidRetireHandler(IPlatformDbContext db) : IRequestHandler<PostEnterpriseApprovalPoliciesPolicyIdGuidRetireCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostEnterpriseApprovalPoliciesPolicyIdGuidRetireCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.policyId, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid policyId, IPlatformDbContext db, CancellationToken ct) { ApprovalPolicy? policy = await db.ApprovalPolicies.SingleOrDefaultAsync(x => x.Id == policyId, ct); if (policy is null) return UseCaseResults.NotFound(); try { policy.Retire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(policy); } catch (InvalidOperationException e) { return Conflict("approval_policy_transition_invalid", e.Message); } }
    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
