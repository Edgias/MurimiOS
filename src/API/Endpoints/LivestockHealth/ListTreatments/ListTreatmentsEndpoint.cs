using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Claims;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.LivestockHealthEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.ListTreatments;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListTreatmentsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/subjects/{subjectId:guid}/treatments", async Task<IResult> (Guid subjectId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockHealth.ListTreatments.ListTreatmentsQuery(subjectId), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
