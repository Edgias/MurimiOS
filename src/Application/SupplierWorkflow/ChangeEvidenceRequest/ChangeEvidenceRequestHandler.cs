using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeEvidenceRequest;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;

public sealed class ChangeEvidenceRequestHandler(IPlatformDbContext db) : IRequestHandler<ChangeEvidenceRequestCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeEvidenceRequestCommand message, CancellationToken cancellationToken) => ChangeEvidenceRequestAsync(message.requestId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeEvidenceRequestAsync(Guid requestId, string command,
        SupplierEvidenceCommandRequest request, IPlatformDbContext db, CancellationToken ct)
    {
        SupplierEvidenceRequest? evidenceRequest = await db.SupplierEvidenceRequests.SingleOrDefaultAsync(x => x.Id == requestId, ct);
        if (evidenceRequest is null) return UseCaseResults.NotFound();
        if (request.EvidenceAttachmentId.HasValue && !await db.EvidenceAttachments.AnyAsync(x => x.Id == request.EvidenceAttachmentId, ct))
            return Bad("evidence_invalid", "The evidence attachment is not available to this tenant.");
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "submit") evidenceRequest.Submit(request.EvidenceAttachmentId, request.SupplierReference, now);
            else if (command == "accept") evidenceRequest.Accept(now, request.Notes);
            else if (command == "reject") evidenceRequest.Reject(now, request.Notes ?? "Rejected");
            else evidenceRequest.Cancel();
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(evidenceRequest);
        }
        catch (InvalidOperationException e) { return Conflict("evidence_request_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
