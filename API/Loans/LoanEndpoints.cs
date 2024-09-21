using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Loans;

internal static class LoanEndpoints
{
    public static RouteGroupBuilder MapLoans(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/loans");
        group.WithTags("Loans");
        group.WithParameterValidation(typeof(LoanRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Loan> loans = await dbContext.Loans.ToListAsync();

            return loans.Select(l => l.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<LoanResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Loans.FindAsync(id) switch
            {
                Loan loan when loan is not null => TypedResults.Ok(loan.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<LoanResponse>> 
            (MurimiOSDbContext dbContext, LoanRequest request) =>
        {
            Loan loan = request.ToEntity();

            dbContext.Add(loan);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/loans/{loan.Id}", loan.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> 
            (MurimiOSDbContext dbContext,
            Guid id, LoanRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Loan? loan = await dbContext.Loans.FindAsync(id);

            if (loan is null)
            {
                return TypedResults.NotFound();
            }

            loan.Update(request);

            dbContext.Update(loan);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Loans
            .Where(l => l.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

