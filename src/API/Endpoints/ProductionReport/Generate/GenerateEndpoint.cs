using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProductionReport.ProductionReportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProductionReport.Generate;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GenerateEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/reports/crop-zones/{cropZoneId:guid}/production.pdf", async Task<IResult> (Guid cropZoneId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProductionReport.Generate.GenerateQuery(cropZoneId), cancellationToken)).ToHttpResult())
            .WithName("GenerateCropZoneProductionReport")
            .WithTags("Reporting")
            .RequireAuthorization(PlatformPermissions.ReportingRead)
            .Produces(StatusCodes.Status200OK, contentType: "application/pdf")
            .Produces(StatusCodes.Status404NotFound);

    }
}
