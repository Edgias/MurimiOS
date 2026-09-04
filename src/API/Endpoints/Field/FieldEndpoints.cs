using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Fields;

internal static class FieldEndpoints
{
    public static RouteGroupBuilder MapFields(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/fields");
        group.WithTags("Fields");
        group.WithParameterValidation(typeof(FieldRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Field> fields = await dbContext.Fields.ToListAsync();

            return fields.Select(f => f.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<FieldResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Fields.FindAsync(id) switch
            {
                Field field when field is not null => TypedResults.Ok(field.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<FieldResponse>> (MurimiOSDbContext dbContext, 
            FieldRequest request) =>
        {
            Field field = request.ToEntity();

            dbContext.Add(field);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/fields/{field.Id}", field.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> 
            (MurimiOSDbContext dbContext,
            Guid id, FieldRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Field? field = await dbContext.Fields.FindAsync(id);

            if (field is null)
            {
                return TypedResults.NotFound();
            }

            field.Update(request);

            dbContext.Update(field);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Fields
            .Where(f => f.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

