using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListInvitations;
using SupplierInvitationStatus = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitationStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListInvitationsEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapGet("/invitations", async Task<IResult> (SupplierInvitationStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ListInvitations.ListInvitationsQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalRead);

    }
}
