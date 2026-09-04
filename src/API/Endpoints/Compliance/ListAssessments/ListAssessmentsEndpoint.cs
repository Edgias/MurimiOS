using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ListAssessments;
using ComplianceAssessmentStatus = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessmentStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListAssessmentsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/assessments", async Task<IResult> (Guid farmId, ComplianceAssessmentStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ListAssessments.ListAssessmentsQuery(farmId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
