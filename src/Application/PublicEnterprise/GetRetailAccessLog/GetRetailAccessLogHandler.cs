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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetRetailAccessLog;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;

using TransparencyAccessLog = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyAccessLog;

public sealed class GetRetailAccessLogHandler(IPlatformDbContext db) : IRequestHandler<GetRetailAccessLogQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetRetailAccessLogQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.tokenId, message.limit, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid? tokenId, int? limit, IPlatformDbContext db, CancellationToken ct) { IQueryable<TransparencyAccessLog> q = db.TransparencyAccessLogs.AsNoTracking(); if (tokenId.HasValue) q = q.Where(x => x.TokenId == tokenId); return UseCaseResults.Ok(await q.OrderByDescending(x => x.OccurredAt).Take(Math.Clamp(limit ?? 200, 1, 1000)).ToArrayAsync(ct)); }

}
