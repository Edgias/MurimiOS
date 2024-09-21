using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.SoilTypes;

internal static class SoilTypeEndpoints
{
    public static RouteGroupBuilder MapSoilTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/soil-types");
        group.WithTags("Soil Types");
        group.WithParameterValidation(typeof(SoilTypeRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<SoilType> soilTypes = await dbContext.SoilTypes.ToListAsync();

            return soilTypes.Select(st => st.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<SoilTypeResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.SoilTypes.FindAsync(id) switch
            {
                SoilType soilType when soilType is not null => TypedResults.Ok(soilType.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<SoilTypeResponse>> (MurimiOSDbContext dbContext, SoilTypeRequest request) =>
        {
            SoilType soilType = request.ToEntity();

            dbContext.Add(soilType);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/soil-types/{soilType.Id}", soilType.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, SoilTypeRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            SoilType? soilType = await dbContext.SoilTypes.FindAsync(id);

            if (soilType is null)
            {
                return TypedResults.NotFound();
            }

            soilType.Update(request);

            dbContext.Update(soilType);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.SoilTypes
            .Where(st => st.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

