using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Customers;

internal static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomers(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/customers");
        group.WithTags("Customers");
        group.WithParameterValidation(typeof(CustomerRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Customer> customers = await dbContext.Customers.ToListAsync();

            return customers.Select(c => c.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<CustomerResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Customers.FindAsync(id) switch
            {
                Customer customer when customer is not null => TypedResults.Ok(customer.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<CustomerResponse>> (MurimiOSDbContext dbContext, 
            CustomerRequest request) =>
        {
            Customer customer = request.ToEntity();

            dbContext.Add(customer);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/customers/{customer.Id}", customer.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, CustomerRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Customer? customer = await dbContext.Customers.FindAsync(id);

            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            customer.Update(request);

            dbContext.Update(customer);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Customers
            .Where(c => c.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

