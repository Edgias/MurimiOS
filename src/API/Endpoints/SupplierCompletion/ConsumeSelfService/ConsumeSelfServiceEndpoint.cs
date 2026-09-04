using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ConsumeSelfService;
internal static class ConsumeSelfServiceEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapPost("/actions/{token}/consume", async Task<IResult> (string token, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.ConsumeSelfService.ConsumeSelfServiceCommand(token), cancellationToken)).ToHttpResult());

    }
}
