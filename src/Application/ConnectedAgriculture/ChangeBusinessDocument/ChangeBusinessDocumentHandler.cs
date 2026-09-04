using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ChangeBusinessDocument;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;

public sealed class ChangeBusinessDocumentHandler(IPlatformDbContext db) : IRequestHandler<ChangeBusinessDocumentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeBusinessDocumentCommand message, CancellationToken cancellationToken) => ChangeBusinessDocumentAsync(message.documentId, message.command, message.request, db, cancellationToken);

    private static async Task<UseCaseResult> ChangeBusinessDocumentAsync(Guid documentId, string command, ExchangeCommandRequest request, IPlatformDbContext db, CancellationToken ct)
    { BusinessDocumentExchange? document = await db.BusinessDocumentExchanges.SingleOrDefaultAsync(x => x.Id == documentId, ct); if (document is null) return UseCaseResults.NotFound(); try { if (command == "validate") document.Validate(); else if (command == "complete") document.Complete(DateTimeOffset.UtcNow, request.AppliedEntityId); else document.Reject(DateTimeOffset.UtcNow, request.Notes ?? "Rejected"); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(document); } catch (InvalidOperationException e) { return Conflict("business_document_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
