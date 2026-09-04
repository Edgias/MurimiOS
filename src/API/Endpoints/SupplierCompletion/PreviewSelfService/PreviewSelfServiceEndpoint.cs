using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.PreviewSelfService;
internal static class PreviewSelfServiceEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapGet("/actions/{token}", async Task<IResult> (string token, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.PreviewSelfService.PreviewSelfServiceQuery(token), cancellationToken)).ToHttpResult());

    }
}
