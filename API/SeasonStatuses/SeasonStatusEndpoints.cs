using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.SeasonStatuses;

internal static class SeasonStatusEndpoints
{
    public static RouteGroupBuilder MapSeasonStatuses(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/season-statuses");
        group.WithTags("Season Statuses");
        group.WithParameterValidation(typeof(SeasonStatusRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<SeasonStatus> seasonStatuses = await dbContext.SeasonStatuses.ToListAsync();

            return seasonStatuses.Select(ss => ss.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<SeasonStatusResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.SeasonStatuses.FindAsync(id) switch
            {
                SeasonStatus seasonStatus when seasonStatus is not null => TypedResults.Ok(seasonStatus.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<SeasonStatusResponse>> (MurimiOSDbContext dbContext, 
            SeasonStatusRequest request) =>
        {
            SeasonStatus seasonStatus = request.ToEntity();

            dbContext.Add(seasonStatus);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/season-statuses/{seasonStatus.Id}", seasonStatus.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, SeasonStatusRequest request) =>
        {
            if (id == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            SeasonStatus? seasonStatus = await dbContext.SeasonStatuses.FindAsync(id);

            if (seasonStatus is null)
            {
                return TypedResults.NotFound();
            }

            seasonStatus.Update(request);

            dbContext.Update(seasonStatus);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.SeasonStatuses
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

