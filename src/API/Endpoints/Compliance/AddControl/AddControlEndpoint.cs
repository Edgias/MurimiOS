using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.AddControl;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.AddControl;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AddControlEndpoint
{
    internal static void Map(IEndpointRouteBuilder standards)
    {
        standards.MapPost("/{standardId:guid}/controls", async Task<IResult> (Guid standardId, CreateComplianceControlRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.AddControl.AddControlCommand(standardId, new global::EdgIQ.MurimiOS.Application.Compliance.AddControl.CreateComplianceControlRequest(request.Id, request.Code, request.Title, request.Description, request.Applicability, request.Weight, request.Critical, request.EvaluatorKey, request.EvaluatorVersion, request.SortOrder)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceManage);

    }
}
