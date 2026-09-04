using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateQualityAssessment;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateQualityAssessment;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateQualityAssessmentEndpoint
{
    internal static void Map(IEndpointRouteBuilder trace)
    {
        trace.MapPost("/quality-assessments", async Task<IResult> (CreateQualityAssessmentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.CreateQualityAssessment.CreateQualityAssessmentCommand(new global::EdgIQ.MurimiOS.Application.Traceability.CreateQualityAssessment.CreateQualityAssessmentRequest(request.Id, request.ObjectType, request.ObjectId, request.Metric, request.NumericResult, request.TextResult, request.Unit, request.Passed, request.AssessedAt, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityWrite);

    }
}
