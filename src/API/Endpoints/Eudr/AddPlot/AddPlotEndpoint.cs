using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.AddPlot;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.AddPlot;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AddPlotEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/cases/{caseId:guid}/plots", async Task<IResult> (Guid caseId, AddEudrPlotRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.AddPlot.AddPlotCommand(caseId, new global::EdgIQ.MurimiOS.Application.Eudr.AddPlot.AddEudrPlotRequest(request.Id, request.FieldId, request.BoundaryVersionId, request.HarvestLotId, request.PlotReference, request.ProductionFrom, request.ProductionTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrManage);

    }
}
