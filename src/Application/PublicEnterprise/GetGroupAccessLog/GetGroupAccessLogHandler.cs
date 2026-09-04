using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupAccessLog;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;

public sealed class GetGroupAccessLogHandler(IPlatformDbContext db) : IRequestHandler<GetGroupAccessLogQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetGroupAccessLogQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.clientId, message.limit, db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(Guid? clientId, int? limit, IPlatformDbContext db, CancellationToken ct) { IQueryable<PublicApiAccessLog> query = db.PublicApiAccessLogs.AsNoTracking(); if (clientId.HasValue) query = query.Where(x => x.ClientId == clientId); return UseCaseResults.Ok(await query.OrderByDescending(x => x.OccurredAt).Take(Math.Clamp(limit ?? 200, 1, 1000)).ToArrayAsync(ct)); }

}
