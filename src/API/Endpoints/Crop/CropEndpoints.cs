using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Crops;

internal static class CropEndpoints
{
    public static RouteGroupBuilder MapCrops(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crops");
        group.WithTags("Crops");
        group.WithParameterValidation(typeof(CropRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Crop> crops = await dbContext.Crops.ToListAsync();

            return crops.Select(c => c.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CropResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Crops.FindAsync(id) switch
            {
                Crop crop when crop is not null => TypedResults.Ok(crop.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CropResponse>> (MurimiOSDbContext dbContext, CropRequest request) =>
        {
            Crop crop = request.ToEntity();

            dbContext.Add(crop);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/crops/{crop.Id}", crop.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CropRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Crop? crop = await dbContext.Crops.FindAsync(id);

            if (crop is null)
            {
                return TypedResults.NotFound();
            }

            crop.Update(request);

            dbContext.Update(crop);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Crops
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

