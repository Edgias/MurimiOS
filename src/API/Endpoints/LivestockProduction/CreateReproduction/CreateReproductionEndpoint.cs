using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateReproductionEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapPost("/subjects/{subjectId:guid}/reproduction-events", async Task<IResult> (Guid subjectId, CreateReproductionEventRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateReproduction.CreateReproductionCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateReproduction.CreateReproductionEventRequest(request.Type, request.Outcome, request.OccurredAt, request.RelatedSubjectId, request.BreedingEventId, request.Method, request.ExpectedDueOn, request.OffspringCount, request.Weight, request.WeightUnit, request.Practitioner, request.Notes, request.Offspring.Select(item => new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateReproduction.CreateOffspringLinkRequest(item.SubjectId, item.Outcome, item.Sex, item.Breed, item.BirthWeight, item.BirthWeightUnit)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
