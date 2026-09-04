using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.ChangeApprovalRequest;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;

public sealed class ChangeApprovalRequestHandler(IPlatformDbContext db) : IRequestHandler<ChangeApprovalRequestCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeApprovalRequestCommand message, CancellationToken cancellationToken) => ChangeApprovalRequestAsync(message.requestId, message.command, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeApprovalRequestAsync(Guid requestId, string command, IPlatformDbContext db, CancellationToken ct)
    { DelegatedApprovalRequest? request = await db.DelegatedApprovalRequests.SingleOrDefaultAsync(x => x.Id == requestId, ct); if (request is null) return UseCaseResults.NotFound(); try { if (command == "cancel") request.Cancel(DateTimeOffset.UtcNow); else request.Expire(DateTimeOffset.UtcNow); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(request); } catch (InvalidOperationException e) { return Conflict("approval_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
