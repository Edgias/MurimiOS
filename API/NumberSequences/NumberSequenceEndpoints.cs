using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.NumberSequences;

internal static class NumberSequenceEndpoints
{
    public static RouteGroupBuilder MapNumberSequences(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/number-sequences");
        group.WithTags("Number Sequences");
        group.WithParameterValidation(typeof(NumberSequenceRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<NumberSequence> numberSequences = await dbContext.NumberSequences.ToListAsync();

            return numberSequences.Select(ns => ns.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<NumberSequenceResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.NumberSequences.FindAsync(id) switch
            {
                NumberSequence numberSequence when numberSequence is not null => TypedResults.Ok(numberSequence.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<NumberSequenceResponse>> (MurimiOSDbContext dbContext, NumberSequenceRequest request) =>
        {
            NumberSequence numberSequence = request.ToEntity();

            dbContext.Add(numberSequence);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/number-sequences/{numberSequence.Id}", numberSequence.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, NumberSequenceRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            NumberSequence? numberSequence = await dbContext.NumberSequences.FindAsync(id);

            if (numberSequence is null)
            {
                return TypedResults.NotFound();
            }

            numberSequence.Update(request);

            dbContext.Update(numberSequence);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.NumberSequences
            .Where(ns => ns.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

