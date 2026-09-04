using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ListMigrationJobs;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;

public sealed class ListMigrationJobsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListMigrationJobsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListMigrationJobsQuery message, CancellationToken cancellationToken) => ListMigrationJobsAsync(message.status, db, routing, cancellationToken);
    private static async Task<UseCaseResult> ListMigrationJobsAsync(TenantMigrationStatus? status, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<TenantMigrationJob> q = db.TenantMigrationJobs.AsNoTracking(); if (status.HasValue) q = q.Where(x => x.Status == status); return UseCaseResults.Ok(await q.OrderByDescending(x => x.RequestedAt).ToArrayAsync(ct)); }


}
