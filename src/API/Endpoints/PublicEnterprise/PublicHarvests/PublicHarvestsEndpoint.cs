using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicHarvests;
internal static class PublicHarvestsEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapGet("/harvest-lots", async Task<IResult> (Guid? farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PublicHarvests.PublicHarvestsQuery(farmId), cancellationToken)).ToHttpResult());

    }
}
