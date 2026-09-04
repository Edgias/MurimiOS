using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateAssessment;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateAssessment;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateAssessmentEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapPost("/assessments", async Task<IResult> (Guid farmId, CreateComplianceAssessmentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.CreateAssessment.CreateAssessmentCommand(farmId, new global::EdgIQ.MurimiOS.Application.Compliance.CreateAssessment.CreateComplianceAssessmentRequest(request.Id, request.StandardVersionId, request.AssessmentCode, request.ScheduledOn, request.Auditor, request.Scope)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceAssess);

    }
}
