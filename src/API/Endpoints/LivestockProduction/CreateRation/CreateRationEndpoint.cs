using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRationEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapPost("/subjects/{subjectId:guid}/ration-plans", async Task<IResult> (Guid subjectId, CreateRationPlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateRation.CreateRationCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateRation.CreateRationPlanRequest(request.Name, request.EffectiveFrom, request.EffectiveTo, request.TargetHeadCount, request.Notes, request.Lines.Select(item => new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateRation.CreateRationLineRequest(item.FeedBatchId, item.FeedName, item.QuantityPerHeadPerDay, item.Unit, item.DryMatterPercent)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
