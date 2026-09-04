using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReceiveRecordsEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/jobs/{jobId:guid}/records", async Task<IResult> (Guid jobId, ReceiveIngestionRecordsRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ReceiveRecords.ReceiveRecordsCommand(jobId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ReceiveRecords.ReceiveIngestionRecordsRequest(request.LeaseId, request.Records.Select(item => new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ReceiveRecords.IngestionRecordRequest(item.Id, item.ExternalRecordId, item.SchemaVersion, item.ObservedAt, item.SourceModifiedAt, item.Quality, item.Payload)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsOperate);

    }
}
