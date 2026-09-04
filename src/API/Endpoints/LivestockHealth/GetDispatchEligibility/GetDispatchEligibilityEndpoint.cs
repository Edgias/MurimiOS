using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Claims;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.LivestockHealthEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.GetDispatchEligibility;
using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetDispatchEligibilityEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/subjects/{subjectId:guid}/dispatch-eligibility", async Task<IResult> (Guid subjectId, LivestockDispatchPurpose purpose, DateTimeOffset? at, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockHealth.GetDispatchEligibility.GetDispatchEligibilityQuery(subjectId, purpose, at), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
