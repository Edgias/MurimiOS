using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetRetailTokens;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

public sealed class GetRetailTokensHandler(IPlatformDbContext db) : IRequestHandler<GetRetailTokensQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetRetailTokensQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.policyId, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid? policyId, IPlatformDbContext db, CancellationToken ct) { IQueryable<TransparencyToken> q = db.TransparencyTokens.AsNoTracking(); if (policyId.HasValue) q = q.Where(x => x.PolicyId == policyId); return UseCaseResults.Ok(await q.OrderByDescending(x => x.IssuedAt).Take(2000).ToArrayAsync(ct)); }

}
