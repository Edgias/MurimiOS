using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.YieldMeasurements;

internal static class YieldMeasurementEndpoints
{
    public static RouteGroupBuilder MapYieldMeasurements(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/yield-measurements");
        group.WithTags("Yield Measurements");
        group.WithParameterValidation(typeof(YieldMeasurementRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<YieldMeasurement> yieldMeasurements = await dbContext.YieldMeasurements.ToListAsync();

            return yieldMeasurements.Select(ym => ym.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<YieldMeasurementResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.YieldMeasurements.FindAsync(id) switch
            {
                YieldMeasurement yieldMeasurement when yieldMeasurement is not null => TypedResults.Ok(yieldMeasurement.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<YieldMeasurementResponse>> (MurimiOSDbContext dbContext, 
            YieldMeasurementRequest request) =>
        {
            YieldMeasurement yieldMeasurement = request.ToEntity();

            dbContext.Add(yieldMeasurement);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/yield-measurements/{yieldMeasurement.Id}", yieldMeasurement.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, YieldMeasurementRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            YieldMeasurement? yieldMeasurement = await dbContext.YieldMeasurements.FindAsync(id);

            if (yieldMeasurement is null)
            {
                return TypedResults.NotFound();
            }

            yieldMeasurement.Update(request);

            dbContext.Update(yieldMeasurement);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.YieldMeasurements
            .Where(ym => ym.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

