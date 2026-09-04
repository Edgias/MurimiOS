using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.CreatePriceRule;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.CreatePriceRule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreatePriceRuleEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/contracts/{contractId:guid}/price-rules", async Task<IResult> (Guid contractId, CreateContractPriceRuleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.CreatePriceRule.CreatePriceRuleCommand(contractId, new global::EdgIQ.MurimiOS.Application.SupplierCompletion.CreatePriceRule.CreateContractPriceRuleRequest(request.Id, request.Priority, request.Method, request.MinimumQuantity, request.MaximumQuantity, request.QualityGrade, request.UnitPrice, request.Currency, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
