using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateMilk;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateMilk;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateMilkEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapPost("/subjects/{subjectId:guid}/milk-records", async Task<IResult> (Guid subjectId, CreateMilkRecordRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateMilk.CreateMilkCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateMilk.CreateMilkRecordRequest(request.MilkedAt, request.Session, request.Quantity, request.DiscardedQuantity, request.Unit, request.FatPercent, request.ProteinPercent, request.QualityGrade, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
