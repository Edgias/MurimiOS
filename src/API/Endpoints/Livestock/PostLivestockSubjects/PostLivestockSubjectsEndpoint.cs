using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Livestock.PostLivestockSubjects;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostLivestockSubjectsEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {

        livestock.MapPost("/subjects", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CreateLivestockSubjectRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Livestock.PostLivestockSubjects.PostLivestockSubjectsCommand(new global::EdgIQ.MurimiOS.Application.Livestock.PostLivestockSubjects.CreateLivestockSubjectRequest(request.Id, request.FarmId, request.Kind, request.Name, request.Species, request.OfficialIdentifier)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
