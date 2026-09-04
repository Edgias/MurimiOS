using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.BinTypes;

internal static class BinTypeEndpoints
{
    public static RouteGroupBuilder MapBinTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/bin-types");
        group.WithTags("Bin Types");
        group.WithParameterValidation(typeof(BinTypeRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<BinType> binTypes = await dbContext.BinTypes.ToListAsync();

            return binTypes.Select(bt => bt.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<BinTypeResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.BinTypes.FindAsync(id) switch
            {
                BinType binType when binType is not null => TypedResults.Ok(binType.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<BinTypeResponse>> (MurimiOSDbContext dbContext, BinTypeRequest request) =>
        {
            BinType binType = request.ToEntity();

            dbContext.Add(binType);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/bin-types/{binType.Id}", binType.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, BinTypeRequest request) =>
        {
            if (id == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            BinType? binType = await dbContext.BinTypes.FindAsync(id);

            if (binType is null)
            {
                return TypedResults.NotFound();
            }

            binType.Update(request);

            dbContext.Update(binType);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.BinTypes
            .Where(bt => bt.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

