using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.ItemCategories;

internal static class ItemCategoryEndpoints
{
    public static RouteGroupBuilder MapItemCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/item-categories");
        group.WithTags("Item Categories");
        group.WithParameterValidation(typeof(ItemCategoryRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<ItemCategory> itemCategories = await dbContext.ItemCategories.ToListAsync();

            return itemCategories.Select(ic => ic.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<ItemCategoryResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.ItemCategories.FindAsync(id) switch
            {
                ItemCategory itemCategory when itemCategory is not null => TypedResults.Ok(itemCategory.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<ItemCategoryResponse>> (MurimiOSDbContext dbContext, ItemCategoryRequest request) =>
        {
            ItemCategory itemCategory = request.ToEntity();

            await dbContext.AddAsync(itemCategory);

            return TypedResults.Created($"/item-categories/{itemCategory.Id}", itemCategory.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, ItemCategoryRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            ItemCategory? itemCategory = await dbContext.ItemCategories.FindAsync(id);

            if (itemCategory is null)
            {
                return TypedResults.NotFound();
            }

            itemCategory.Update(request);

            dbContext.Update(itemCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.ItemCategories
            .Where(ic => ic.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

