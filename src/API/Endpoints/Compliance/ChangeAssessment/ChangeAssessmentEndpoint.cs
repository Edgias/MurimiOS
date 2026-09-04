using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeAssessment;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeAssessmentEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapPost("/assessments/{assessmentId:guid}/{command:regex(^start|submit|complete|cancel$)}", async Task<IResult> (Guid farmId, Guid assessmentId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ChangeAssessment.ChangeAssessmentCommand(farmId, assessmentId, command), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceAssess);

    }
}
