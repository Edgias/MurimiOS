using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Seasons;

internal static class SeasonEndpoints
{
    public static RouteGroupBuilder MapSeasons(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/seasons");
        group.WithTags("Seasons");
        group.WithParameterValidation(typeof(SeasonRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Season> seasons = await dbContext.Seasons.ToListAsync();

            return seasons.Select(s => s.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<SeasonResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Seasons.FindAsync(id) switch
            {
                Season season when season is not null => TypedResults.Ok(season.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<SeasonResponse>> (MurimiOSDbContext dbContext, 
            SeasonRequest request) =>
        {
            Season season = request.ToEntity();

            dbContext.Add(season);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/seasons/{season.Id}", season.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, SeasonRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Season? season = await dbContext.Seasons.FindAsync(id);

            if (season is null)
            {
                return TypedResults.NotFound();
            }

            season.Update(request);

            dbContext.Update(season);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Seasons
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

