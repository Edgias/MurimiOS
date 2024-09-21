using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.CropCategories;

internal static class CropCategoryEndpoints
{
    public static RouteGroupBuilder MapCropCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-categories");
        group.WithTags("Crop Categories");
        group.WithParameterValidation(typeof(CropCategoryRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<CropCategory> cropCategories = await dbContext.CropCategories.ToListAsync();

            return cropCategories.Select(cc => cc.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CropCategoryResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.CropCategories.FindAsync(id) switch
            {
                CropCategory cropCategory when cropCategory is not null => TypedResults.Ok(cropCategory.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CropCategoryResponse>> (MurimiOSDbContext dbContext, CropCategoryRequest request) =>
        {
            CropCategory cropCategory = request.ToEntity();

            dbContext.Add(cropCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/crop-categories/{cropCategory.Id}", cropCategory.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CropCategoryRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            CropCategory? cropCategory = await dbContext.CropCategories.FindAsync(id);

            if (cropCategory is null)
            {
                return TypedResults.NotFound();
            }

            cropCategory.Update(request);

            dbContext.Update(cropCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.CropCategories
            .Where(cc => cc.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

