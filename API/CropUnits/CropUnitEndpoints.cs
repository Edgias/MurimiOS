using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.CropUnits;

internal static class CropUnitEndpoints
{
    public static RouteGroupBuilder MapCropUnits(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-units");
        group.WithTags("Crop Units");
        group.WithParameterValidation(typeof(CropUnitRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<CropUnit> cropUnits = await dbContext.CropUnits.ToListAsync();

            return cropUnits.Select(cu => cu.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CropUnitResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.CropUnits.FindAsync(id) switch
            {
                CropUnit cropUnit when cropUnit is not null => TypedResults.Ok(cropUnit.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CropUnitResponse>> (MurimiOSDbContext dbContext, CropUnitRequest request) =>
        {
            CropUnit cropUnit = request.ToEntity();

            dbContext.Add(cropUnit);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/crop-units/{cropUnit.Id}", cropUnit.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CropUnitRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            CropUnit? cropUnit = await dbContext.CropUnits.FindAsync(id);

            if (cropUnit is null)
            {
                return TypedResults.NotFound();
            }

            cropUnit.Update(request);

            dbContext.Update(cropUnit);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.CropUnits
            .Where(cu => cu.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

