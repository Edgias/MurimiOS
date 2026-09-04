using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Locations;

internal static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocations(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/locations");
        group.WithTags("Locations");
        group.WithParameterValidation(typeof(LocationRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Location> locations = await dbContext.Locations.ToListAsync();

            return locations.Select(l => l.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<LocationResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Locations.FindAsync(id) switch
            {
                Location location when location is not null => TypedResults.Ok(location.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<LocationResponse>> 
            (MurimiOSDbContext dbContext, LocationRequest request) =>
        {
            Location location = request.ToEntity();

            dbContext.Add(location);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/locations/{location.Id}", location.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> 
            (MurimiOSDbContext dbContext,
            Guid id, LocationRequest request) =>
        {
            if (id == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            Location? location = await dbContext.Locations.FindAsync(id);

            if (location is null)
            {
                return TypedResults.NotFound();
            }

            location.Update(request);

            dbContext.Update(location);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Locations
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

