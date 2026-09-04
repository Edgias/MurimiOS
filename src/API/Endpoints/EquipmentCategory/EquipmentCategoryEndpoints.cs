using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.EquipmentCategories;

internal static class EquipmentCategoryEndpoints
{
    public static RouteGroupBuilder MapMachineryCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/equipment-categories");
        group.WithTags("Equipment Categories");
        group.WithParameterValidation(typeof(EquipmentCategoryRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<EquipmentCategory> equipmentCategories = await dbContext.EquipmentCategories.ToListAsync();

            return equipmentCategories.Select(ec => ec.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<EquipmentCategoryResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.EquipmentCategories.FindAsync(id) switch
            {
                EquipmentCategory equipmentCategory when equipmentCategory is not null => TypedResults.Ok(equipmentCategory.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<EquipmentCategoryResponse>> (MurimiOSDbContext dbContext, EquipmentCategoryRequest request) =>
        {
            EquipmentCategory equipmentCategory = request.ToEntity();

            dbContext.Add(equipmentCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/equipment-categories/{equipmentCategory.Id}", equipmentCategory.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, EquipmentCategoryRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            EquipmentCategory? equipmentCategory = await dbContext.EquipmentCategories.FindAsync(id);

            if (equipmentCategory is null)
            {
                return TypedResults.NotFound();
            }

            equipmentCategory.Update(request);

            dbContext.Update(equipmentCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.EquipmentCategories
            .Where(eq => eq.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

