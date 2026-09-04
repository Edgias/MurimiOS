using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.FieldMeasurements;

internal static class FieldMeasurementEndpoints
{
    public static RouteGroupBuilder MapFieldMeasurements(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/field-measurements");
        group.WithTags("Field Measurements");
        group.WithParameterValidation(typeof(FieldMeasurementRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<FieldMeasurement> fieldMeasurements = await dbContext.FieldMeasurements.ToListAsync();

            return fieldMeasurements.Select(fm => fm.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<FieldMeasurementResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.FieldMeasurements.FindAsync(id) switch
            {
                FieldMeasurement fieldMeasurement when fieldMeasurement is not null => TypedResults.Ok(fieldMeasurement.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<FieldMeasurementResponse>> (MurimiOSDbContext dbContext, FieldMeasurementRequest request) =>
        {
            FieldMeasurement fieldMeasurement = request.ToEntity();

            dbContext.Add(fieldMeasurement);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/field-measurements/{fieldMeasurement.Id}", fieldMeasurement.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, FieldMeasurementRequest rquest) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            FieldMeasurement? fieldMeasurement = await dbContext.FieldMeasurements.FindAsync(id);

            if (fieldMeasurement is null)
            {
                return TypedResults.NotFound();
            }

            fieldMeasurement.Update(rquest);

            dbContext.Update(fieldMeasurement);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.FieldMeasurements
            .Where(fm => fm.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

