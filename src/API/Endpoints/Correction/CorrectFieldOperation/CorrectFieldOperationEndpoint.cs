using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectFieldOperation;
using static EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectFieldOperation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CorrectFieldOperationEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/field-operations/{operationId:guid}/corrections", async Task<IResult> (Guid operationId, CorrectFieldOperationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Correction.CorrectFieldOperation.CorrectFieldOperationCommand(operationId, new global::EdgIQ.MurimiOS.Application.Correction.CorrectFieldOperation.CorrectFieldOperationRequest(request.CorrectionId, request.ReplacementId, request.CommandId, request.OperationType, request.OccurredAt, request.Notes, request.Reason)), cancellationToken)).ToHttpResult())
            .WithTags("Corrections")
            .RequireAuthorization(PlatformPermissions.ProductionWrite);

    }
}
