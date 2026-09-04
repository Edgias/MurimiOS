using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateCompany;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateCompany;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCompanyEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/legal-companies", async Task<IResult> (CreateLegalCompanyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateCompany.CreateCompanyCommand(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateCompany.CreateLegalCompanyRequest(request.Id, request.OrganisationNodeId, request.CompanyCode, request.LegalName, request.RegistrationNumber, request.TaxIdentifier, request.CountryCode, request.FunctionalCurrency, request.EffectiveFrom)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AccessManage);

    }
}
