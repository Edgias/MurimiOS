using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListRiskAssessments;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListRiskAssessmentsEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapGet("/risk-assessments", async Task<IResult> (Guid? relationshipId, Guid? hostedSupplierId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.ListRiskAssessments.ListRiskAssessmentsQuery(relationshipId, hostedSupplierId, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalRead);

    }
}
