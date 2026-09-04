using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Compliance.EvaluateControl;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.EvaluateControl;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class EvaluateControlEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapPut("/assessments/{assessmentId:guid}/controls/{controlId:guid}", async Task<IResult> (Guid farmId, Guid assessmentId, Guid controlId, EvaluateComplianceControlRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.EvaluateControl.EvaluateControlCommand(farmId, assessmentId, controlId, new global::EdgIQ.MurimiOS.Application.Compliance.EvaluateControl.EvaluateComplianceControlRequest(request.Result, request.ScorePercent, request.Notes, request.EvidenceAttachmentIds)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceAssess);

    }
}
