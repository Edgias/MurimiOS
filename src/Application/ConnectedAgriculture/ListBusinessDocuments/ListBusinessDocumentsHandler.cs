using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListBusinessDocuments;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;

using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using ExchangeStatus = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.ExchangeStatus;

public sealed class ListBusinessDocumentsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListBusinessDocumentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListBusinessDocumentsQuery message, CancellationToken cancellationToken) => ListBusinessDocumentsAsync(message.farmId, message.status, message.type, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListBusinessDocumentsAsync(Guid farmId, ExchangeStatus? status, BusinessDocumentType? type, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound(); IQueryable<BusinessDocumentExchange> query = db.BusinessDocumentExchanges.AsNoTracking().Where(x => x.FarmId == farmId); if (status.HasValue) query = query.Where(x => x.Status == status); if (type.HasValue) query = query.Where(x => x.DocumentType == type); return UseCaseResults.Ok(await query.OrderByDescending(x => x.OccurredAt).ToArrayAsync(ct)); }


}
