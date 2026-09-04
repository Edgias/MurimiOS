using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.OwnershipTypes;

internal static class OwnershipTypeEndpoints
{
    public static RouteGroupBuilder MapOwnershipTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/ownership-types");
        group.WithTags("Ownership Types");
        group.WithParameterValidation(typeof(OwnershipTypeRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<OwnershipType> ownershipTypes = await dbContext.OwnershipTypes.ToListAsync();

            return ownershipTypes.Select(ot => ot.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<OwnershipTypeResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.OwnershipTypes.FindAsync(id) switch
            {
                OwnershipType ownershipType when ownershipType is not null => TypedResults.Ok(ownershipType.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<OwnershipTypeResponse>> (MurimiOSDbContext dbContext, OwnershipTypeRequest request) =>
        {
            OwnershipType ownershipType = request.ToEntity();

            await dbContext.AddAsync(ownershipType);

            return TypedResults.Created($"/ownership-types/{ownershipType.Id}", ownershipType.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, OwnershipTypeRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            OwnershipType? ownershipType = await dbContext.OwnershipTypes.FindAsync(id);

            if (ownershipType is null)
            {
                return TypedResults.NotFound();
            }

            ownershipType.Update(request);

            dbContext.Update(ownershipType);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.OwnershipTypes
            .Where(ot => ot.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

