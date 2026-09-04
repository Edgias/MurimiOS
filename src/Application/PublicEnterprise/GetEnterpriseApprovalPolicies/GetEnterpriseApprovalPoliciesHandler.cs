using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetEnterpriseApprovalPolicies;


public sealed class GetEnterpriseApprovalPoliciesHandler(IPlatformDbContext db) : IRequestHandler<GetEnterpriseApprovalPoliciesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetEnterpriseApprovalPoliciesQuery message, CancellationToken cancellationToken) => ExecuteAsync(db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(IPlatformDbContext db, CancellationToken ct) =>UseCaseResults.Ok(await db.ApprovalPolicies.AsNoTracking().OrderBy(x => x.Code).ToArrayAsync(ct));

}
