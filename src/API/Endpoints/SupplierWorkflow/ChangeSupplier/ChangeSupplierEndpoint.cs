using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeSupplier;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using SupplierCommandRequest = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.Shared.SupplierCommandRequest;
internal static class ChangeSupplierEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/suppliers/{supplierId:guid}/{command:regex(^suspend|reactivate$)}", async Task<IResult> (Guid supplierId, string command, SupplierCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeSupplier.ChangeSupplierCommand(supplierId, command, new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeSupplier.SupplierCommandRequest(request.Notes, request.ClaimedTenantId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
