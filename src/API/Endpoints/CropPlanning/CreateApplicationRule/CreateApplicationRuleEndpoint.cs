using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateApplicationRule;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateApplicationRule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateApplicationRuleEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/inputs/{inputId:guid}/application-rules", async Task<IResult> (Guid inputId, CreateProductApplicationRuleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.CreateApplicationRule.CreateApplicationRuleCommand(inputId, new global::EdgIQ.MurimiOS.Application.CropPlanning.CreateApplicationRule.CreateProductApplicationRuleRequest(request.Id, request.Crop, request.Jurisdiction, request.MaximumRatePerHa, request.RateUnit, request.ReEntryHours, request.PreHarvestIntervalDays, request.EffectiveFrom, request.EffectiveTo, request.AuthorizationReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
