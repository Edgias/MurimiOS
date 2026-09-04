using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Livestock.GetLivestockSubjects;
using LivestockSubjectResponse = global::EdgIQ.MurimiOS.API.Endpoints.Livestock.Shared.LivestockSubjectResponse;

using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetLivestockSubjectsEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {

        livestock.MapGet("/subjects", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Livestock.GetLivestockSubjects.GetLivestockSubjectsQuery(), cancellationToken)).ToHttpResult()).Produces<LivestockSubjectResponse[]>(200).RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
