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
namespace EdgIQ.MurimiOS.Application.Epcis.CaptureStatus;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;

public sealed class CaptureStatusHandler(IResponseMetadata response, IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CaptureStatusQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CaptureStatusQuery message, CancellationToken cancellationToken) => CaptureStatusAsync(message.captureId, response, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CaptureStatusAsync(Guid captureId, IResponseMetadata response,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        EpcisExchange? exchange = await db.EpcisExchanges.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == captureId && x.Direction == EpcisExchangeDirection.Capture, ct);
        if (exchange is null || !await scope.CanAccessFarmAsync(exchange.FarmId, ct)) return UseCaseResults.NotFound();
        StandardHeaders(response);
        return UseCaseResults.Ok(Status(exchange));
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
