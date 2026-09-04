using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.ListCaptures;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;

public sealed class ListCapturesHandler(IResponseMetadata response, IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListCapturesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListCapturesQuery message, CancellationToken cancellationToken) => ListCapturesAsync(message.farmId, message.perPage, response, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListCapturesAsync(Guid farmId, int? perPage, IResponseMetadata response,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        int take = Math.Clamp(perPage ?? 30, 1, 100);
        EpcisExchange[] rows = await db.EpcisExchanges.AsNoTracking().Where(x => x.FarmId == farmId
            && x.Direction == EpcisExchangeDirection.Capture).OrderByDescending(x => x.ExchangedAt).Take(take).ToArrayAsync(ct);
        StandardHeaders(response);
        return UseCaseResults.Ok(rows.Select(Status));
    }


    private static object Status(EpcisExchange x) => new
    {
        captureID = x.Id, createdAt = x.ExchangedAt, finishedAt = x.ExchangedAt,
        running = false, success = true, captureErrorBehaviour = "rollback", errors = Array.Empty<object>(),
        x.DocumentId, x.Sha256, x.EventCount
    };


    private static void StandardHeaders(IResponseMetadata response)
    {
        response["GS1-EPCIS-Version"] = EpcisInterop.EpcisVersion;
        response["GS1-CBV-Version"] = EpcisInterop.CbvVersion;
        response["GS1-Extensions"] = "https://murimios.io/epcis/";
    }

}
