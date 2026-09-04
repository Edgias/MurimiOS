using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CloseCompany;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CloseCompany;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CloseCompanyEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/legal-companies/{id:guid}/close", async Task<IResult> (Guid id, CloseLegalCompanyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CloseCompany.CloseCompanyCommand(id, new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CloseCompany.CloseLegalCompanyRequest(request.EffectiveTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccessManage);

    }
}
