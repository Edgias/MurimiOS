using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateTransformationEndpoint
{
    internal static void Map(IEndpointRouteBuilder trace)
    {
        trace.MapPost("/transformations", async Task<IResult> (CreateTransformationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.CreateTransformation.CreateTransformationCommand(new global::EdgIQ.MurimiOS.Application.Traceability.CreateTransformation.CreateTransformationRequest(request.Id, request.Inputs.Select(item => new global::EdgIQ.MurimiOS.Application.Traceability.CreateTransformation.TransformationInput(item.ObjectType, item.ObjectId, item.Quantity, item.Unit)).ToArray(), request.Outputs.Select(item => new global::EdgIQ.MurimiOS.Application.Traceability.CreateTransformation.TransformationOutput(item.Id, item.LotCode, item.Quantity, item.Unit, item.StorageLocation)).ToArray(), request.DeclaredLoss, request.LossUnit, request.OccurredAt, request.LocationId, request.BusinessStep, request.Disposition, request.TransactionReference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityWrite);

    }
}
