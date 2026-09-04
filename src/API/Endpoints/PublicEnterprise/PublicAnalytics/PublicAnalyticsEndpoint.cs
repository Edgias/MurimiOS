using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicAnalytics;
internal static class PublicAnalyticsEndpoint
{
    internal static void Map(IEndpointRouteBuilder publicApi)
    {
        publicApi.MapGet("/analytics/facts", async Task<IResult> (Guid? farmId, string? metricCode, DateOnly? from, DateOnly? to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PublicAnalytics.PublicAnalyticsQuery(farmId, metricCode, from, to), cancellationToken)).ToHttpResult());

    }
}
