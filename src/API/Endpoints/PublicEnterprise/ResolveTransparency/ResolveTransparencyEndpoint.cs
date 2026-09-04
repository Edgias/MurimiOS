using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ResolveTransparency;
internal static class ResolveTransparencyEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapGet("/transparency/{token}", async Task<IResult> (string token, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.ResolveTransparency.ResolveTransparencyQuery(token), cancellationToken)).ToHttpResult());

    }
}
