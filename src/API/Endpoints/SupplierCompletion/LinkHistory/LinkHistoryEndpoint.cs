using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.LinkHistory;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class LinkHistoryEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapGet("/suppliers/{supplierId:guid}/link-history", async Task<IResult> (Guid supplierId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.LinkHistory.LinkHistoryQuery(supplierId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalRead);

    }
}
