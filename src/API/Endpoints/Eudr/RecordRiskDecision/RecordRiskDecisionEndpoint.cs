using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordRiskDecision;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordRiskDecision;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordRiskDecisionEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/cases/{caseId:guid}/risk-decisions", async Task<IResult> (Guid caseId, RecordEudrRiskDecisionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.RecordRiskDecision.RecordRiskDecisionCommand(caseId, new global::EdgIQ.MurimiOS.Application.Eudr.RecordRiskDecision.RecordEudrRiskDecisionRequest(request.Id, request.Conclusion, request.LegalProductionConfirmed, request.Rationale, request.Mitigation, request.LegalityEvidence)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrAssess);

    }
}
