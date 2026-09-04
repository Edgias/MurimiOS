using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicTrace;
internal static class PublicTraceEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapGet("/harvest-lots/{harvestLotId:guid}/trace", async Task<IResult> (Guid harvestLotId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PublicTrace.PublicTraceQuery(harvestLotId), cancellationToken)).ToHttpResult());

    }
}
