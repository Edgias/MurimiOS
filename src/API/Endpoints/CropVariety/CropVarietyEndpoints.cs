using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.CropVarieties;

internal static class CropVarietyEndpoints
{
    public static RouteGroupBuilder MapCropVarieties(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-varieties");
        group.WithTags("Crop Varieties");
        group.WithParameterValidation(typeof(CropVarietyRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<CropVariety> cropVarieties = await dbContext.CropVarieties.ToListAsync();

            return cropVarieties.Select(cv => cv.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CropVarietyResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.CropVarieties.FindAsync(id) switch
            {
                CropVariety cropVariety when cropVariety is not null => TypedResults.Ok(cropVariety.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CropVarietyResponse>> (MurimiOSDbContext dbContext, 
            CropVarietyRequest request) =>
        {
            CropVariety entity = request.ToEntity();

            dbContext.Add(entity);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/crop-varieties/{entity.Id}", entity.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CropVarietyRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            CropVariety? entity = await dbContext.CropVarieties.FindAsync(id);

            if (entity is null)
            {
                return TypedResults.NotFound();
            }

            entity.Update(request);

            dbContext.Update(entity);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.CropVarieties
            .Where(cv => cv.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

