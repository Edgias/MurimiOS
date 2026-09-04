using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ConvertQuantityEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/reference-data/units/convert", async Task<IResult> (ConvertMeasurementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.ConvertQuantity.ConvertQuantityCommand(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.ConvertQuantity.ConvertMeasurementRequest(request.Value, request.FromUnit, request.ToUnit)), cancellationToken)).ToHttpResult())
            .WithTags("Reference data")
            .RequireAuthorization(PlatformPermissions.ReferenceDataRead);

    }
}
