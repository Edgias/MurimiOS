using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Currencies;

internal static class CurrencyEndpoints
{
    public static RouteGroupBuilder MapCurrencies(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/currencies");
        group.WithTags("Currencies");
        group.WithParameterValidation(typeof(CurrencyRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Currency> currencies = await dbContext.Currencies.ToListAsync();

            return currencies.Select(c => c.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CurrencyResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Currencies.FindAsync(id) switch
            {
                Currency currency when currency is not null => TypedResults.Ok(currency.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CurrencyResponse>> (MurimiOSDbContext dbContext, CurrencyRequest request) =>
        {
            Currency currency = request.ToEntity();

            dbContext.Add(currency);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/currencies/{currency.Id}", currency.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CurrencyRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Currency? currency = await dbContext.Currencies.FindAsync(id);

            if (currency is null)
            {
                return TypedResults.NotFound();
            }

            currency.Update(request);

            dbContext.Update(currency);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Currencies
            .Where(cv => cv.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

