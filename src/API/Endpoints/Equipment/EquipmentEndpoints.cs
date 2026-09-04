using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Equipments;

internal static class EquipmentEndpoints
{
    public static RouteGroupBuilder MapMachineryCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/equipments");
        group.WithTags("Equipments");
        group.WithParameterValidation(typeof(EquipmentRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Equipment> equipments = await dbContext.Equipments.ToListAsync();

            return equipments.Select(e => e.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<EquipmentResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Equipments.FindAsync(id) switch
            {
                Equipment equipment when equipment is not null => TypedResults.Ok(equipment.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<EquipmentResponse>> (MurimiOSDbContext dbContext, EquipmentRequest request) =>
        {
            Equipment equipment = request.ToEntity();

            dbContext.Add(equipment);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/equipments/{equipment.Id}", equipment.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, EquipmentRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Equipment? equipment = await dbContext.Equipments.FindAsync(id);

            if (equipment is null)
            {
                return TypedResults.NotFound();
            }

            equipment.Update(request);

            dbContext.Update(equipment);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Equipments
            .Where(e => e.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

