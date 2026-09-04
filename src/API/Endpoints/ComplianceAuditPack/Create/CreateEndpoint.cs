using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create;
using EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create;
using EdgIQ.MurimiOS.API.Endpoints.Notification.Create;
using static EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.ComplianceAuditPackEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapPost("/assessments/{assessmentId:guid}/audit-packs", async Task<IResult> (Guid farmId, Guid assessmentId, CreateComplianceAuditPackRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ComplianceAuditPack.Create.CreateCommand(farmId, assessmentId, new global::EdgIQ.MurimiOS.Application.ComplianceAuditPack.Create.CreateComplianceAuditPackRequest(request.Id, request.ScopeStatement, request.IncludeEvidenceFiles)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ComplianceExport);

    }
}
