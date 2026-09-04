using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordScreening;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordScreening;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordScreeningEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/plots/{plotId:guid}/screenings", async Task<IResult> (Guid plotId, RecordEudrScreeningRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.RecordScreening.RecordScreeningCommand(plotId, new global::EdgIQ.MurimiOS.Application.Eudr.RecordScreening.RecordEudrScreeningRequest(request.Id, request.ProviderCode, request.DatasetName, request.DatasetVersion, request.ObservationFrom, request.ObservationThrough, request.CoveragePercent, request.PostCutoffTreeCoverLossHa, request.DeforestationOverlapHa, request.ConfidencePercent, request.SourceReference, request.EvidenceAttachmentId, request.ProviderResult)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrScreen);

    }
}
