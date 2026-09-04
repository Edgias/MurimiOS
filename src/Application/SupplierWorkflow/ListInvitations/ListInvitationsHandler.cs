using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ListInvitations;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
using SupplierInvitationStatus = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitationStatus;

public sealed class ListInvitationsHandler(IPlatformDbContext db) : IRequestHandler<ListInvitationsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListInvitationsQuery message, CancellationToken cancellationToken) => ListInvitationsAsync(message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListInvitationsAsync(SupplierInvitationStatus? status, IPlatformDbContext db, CancellationToken ct)
    {
        IQueryable<SupplierInvitation> query = db.SupplierInvitations.AsNoTracking();
        if (status.HasValue) query = query.Where(x => x.Status == status);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.InvitedAt).ToArrayAsync(ct));
    }


}
