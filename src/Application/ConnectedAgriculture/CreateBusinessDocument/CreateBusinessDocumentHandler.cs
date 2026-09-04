using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.CreateBusinessDocument;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;

using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using ExchangeDirection = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.ExchangeDirection;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;

public sealed class CreateBusinessDocumentHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateBusinessDocumentCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateBusinessDocumentCommand message, CancellationToken cancellationToken) => CreateBusinessDocumentAsync(message.farmId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateBusinessDocumentAsync(Guid farmId, CreateBusinessDocumentRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await FarmAllowed(farmId, db, scope, ct)) return UseCaseResults.NotFound();
        if (!await db.IntegrationConnections.AnyAsync(x => x.Id == request.ConnectionId && x.Status == IntegrationConnectionStatus.Active && (x.Category == IntegrationCategory.Erp || x.Category == IntegrationCategory.Accounting), ct)) return Bad("business_connection_invalid", "An active ERP or accounting connection is required.");
        try { var document = new BusinessDocumentExchange(Id(request.Id), request.ConnectionId, farmId, request.DocumentType, request.Direction, request.ExternalDocumentId, request.SchemaVersion, request.MappingVersion, request.Payload.GetRawText(), request.PayloadSha256, request.OccurredAt, DateTimeOffset.UtcNow); db.BusinessDocumentExchanges.Add(document); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/connected/business-documents/{document.Id}", document); }
        catch (ArgumentException e) { return Bad("business_document_invalid", e.Message); }
    }


    private static async Task<bool> FarmAllowed(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct) => await scope.CanAccessFarmAsync(farmId, ct) && await db.Farms.AnyAsync(x => x.Id == farmId, ct);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
