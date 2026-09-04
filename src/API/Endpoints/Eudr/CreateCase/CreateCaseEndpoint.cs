using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.CreateCase;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.CreateCase;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCaseEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/farms/{farmId:guid}/cases", async Task<IResult> (Guid farmId, CreateEudrCaseRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.CreateCase.CreateCaseCommand(farmId, new global::EdgIQ.MurimiOS.Application.Eudr.CreateCase.CreateEudrCaseRequest(request.Id, request.CaseNumber, request.Commodity, request.ProductDescription, request.HsCode, request.Quantity, request.Unit, request.ProductionCountry, request.ProductionFrom, request.ProductionTo, request.OperatorName, request.OperatorRegistrationNumber, request.OperatorAddress, request.Market)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrManage);

    }
}
