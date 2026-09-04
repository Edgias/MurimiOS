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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.GetGroupClients;


public sealed class GetGroupClientsHandler(IPlatformDbContext db) : IRequestHandler<GetGroupClientsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetGroupClientsQuery message, CancellationToken cancellationToken) => ExecuteAsync(db, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(IPlatformDbContext db, CancellationToken ct) =>UseCaseResults.Ok(await db.PublicApiClients.AsNoTracking().OrderBy(x => x.Name).ToArrayAsync(ct));

}
