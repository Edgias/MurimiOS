using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.CropProductions;

internal static class CropProductionEndpoints
{
    public static RouteGroupBuilder MapCropProductions(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-productions");
        group.WithTags("Crop Productions");
        group.WithParameterValidation(typeof(CropProductionRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<CropProduction> cropProductions = await dbContext.CropProductions.ToListAsync();

            return cropProductions.Select(cp => cp.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CropProductionResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.CropProductions.FindAsync(id) switch
            {
                CropProduction cropProduction when cropProduction is not null => TypedResults.Ok(cropProduction.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CropProductionResponse>> (MurimiOSDbContext dbContext, CropProductionRequest request) =>
        {
            CropProduction cropProduction = request.ToEntity();

            dbContext.Add(cropProduction);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/crop-productions/{cropProduction.Id}", cropProduction.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CropProductionRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            CropProduction? cropProduction = await dbContext.CropProductions.FindAsync(id);

            if (cropProduction is null)
            {
                return TypedResults.NotFound();
            }

            cropProduction.Update(request);

            dbContext.Update(cropProduction);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.CropProductions
            .Where(cp => cp.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

