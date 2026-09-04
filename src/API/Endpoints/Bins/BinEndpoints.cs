using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Bins;

internal static class BinEndpoints
{
    public static RouteGroupBuilder MapBins(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/bins");
        group.WithTags("Bins");
        group.WithParameterValidation(typeof(BinRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            IReadOnlyList<Bin> bins = await dbContext.Bins.ToListAsync();

            return bins.Select(b => b.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<BinResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Bins.FindAsync(id) switch
            {
                Bin bin when bin is not null => TypedResults.Ok(bin.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<BinResponse>> (MurimiOSDbContext dbContext, BinRequest request) =>
        {
            Bin bin = request.ToEntity();

            dbContext.Add(bin);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/bins/{bin.Id}", bin.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, BinRequest request) =>
        {
            if (id == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            Bin? bin = await dbContext.Bins.FindAsync(id);

            if (bin is null)
            {
                return TypedResults.NotFound();
            }

            bin.Update(request);

            dbContext.Update(bin);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Bins
            .Where(b => b.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

