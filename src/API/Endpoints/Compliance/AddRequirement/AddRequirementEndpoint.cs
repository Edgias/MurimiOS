using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.AddRequirement;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.AddRequirement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AddRequirementEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapPost("/{standardId:guid}/controls/{controlId:guid}/requirements", async Task<IResult> (Guid standardId, Guid controlId, CreateComplianceRequirementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.AddRequirement.AddRequirementCommand(standardId, controlId, new global::EdgIQ.MurimiOS.Application.Compliance.AddRequirement.CreateComplianceRequirementRequest(request.Id, request.Code, request.Name, request.EvidenceType, request.Required, request.MinimumCount, request.MaximumAgeDays, request.Instructions)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceManage);

    }
}
