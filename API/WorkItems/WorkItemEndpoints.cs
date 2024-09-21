using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.WorkItems;

internal static class WorkItemEndpoints
{
    public static RouteGroupBuilder MapWorkItems(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-items");
        group.WithTags("Work Items");
        group.WithParameterValidation(typeof(WorkItemRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<WorkItem> workItems = await dbContext.WorkItems.ToListAsync();

            return workItems.Select(wi => wi.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<WorkItemResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.WorkItems.FindAsync(id) switch
            {
                WorkItem workItem when workItem is not null => TypedResults.Ok(workItem.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<WorkItemResponse>> (MurimiOSDbContext dbContext, 
            WorkItemRequest request) =>
        {
            WorkItem workItem = request.ToEntity();

            dbContext.Add(workItem);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/work-items/{workItem.Id}", workItem.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, WorkItemRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            WorkItem? workItem = await dbContext.WorkItems.FindAsync(id);

            if (workItem is null)
            {
                return TypedResults.NotFound();
            }

            workItem.Update(request);

            dbContext.Update(workItem);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.WorkItems
            .Where(wi => wi.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

