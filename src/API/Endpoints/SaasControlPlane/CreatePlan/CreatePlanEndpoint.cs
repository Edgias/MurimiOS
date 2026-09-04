using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreatePlan;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreatePlan;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreatePlan;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;
internal static class CreatePlanEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/plans", async Task<IResult> (CreateSaasPlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreatePlan.CreatePlanCommand(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.CreatePlan.CreateSaasPlanRequest(request.Id, request.SeriesId, request.Code, request.Name, request.PlanVersion, request.BillingInterval, request.BasePrice, request.Currency, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}
