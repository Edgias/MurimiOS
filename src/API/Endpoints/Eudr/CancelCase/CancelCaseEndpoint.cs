using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.CancelCase;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.CancelCase;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CancelCaseEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/cases/{caseId:guid}/cancel", async Task<IResult> (Guid caseId, CancelEudrCaseRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.CancelCase.CancelCaseCommand(caseId, new global::EdgIQ.MurimiOS.Application.Eudr.CancelCase.CancelEudrCaseRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrManage);

    }
}
