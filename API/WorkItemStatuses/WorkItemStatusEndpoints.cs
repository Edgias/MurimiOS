using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.WorkItemStatuses;

internal static class WorkItemStatusEndpoints
{
    public static RouteGroupBuilder MapWorkItemStatuses(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-statuses");
        group.WithTags("Work Item Statuses");
        group.WithParameterValidation(typeof(WorkItemStatusRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<WorkItemStatus> workItemStatuses = await dbContext.WorkItemStatuses.ToListAsync();

            return workItemStatuses.Select(wis => wis.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<WorkItemStatusResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.WorkItemStatuses.FindAsync(id) switch
            {
                WorkItemStatus workItemStatus when workItemStatus is not null => TypedResults.Ok(workItemStatus.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<WorkItemStatusResponse>> (MurimiOSDbContext dbContext, 
            WorkItemStatusRequest request) =>
        {
            WorkItemStatus workItemStatus = request.ToEntity();

            dbContext.Add(workItemStatus);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/work-item-statuses/{workItemStatus.Id}", workItemStatus.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, WorkItemStatusRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            WorkItemStatus? workItemStatus = await dbContext.WorkItemStatuses.FindAsync(id);

            if (workItemStatus is null)
            {
                return TypedResults.NotFound();
            }

            workItemStatus.Update(request);

            dbContext.Update(workItemStatus);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.WorkItemStatuses
            .Where(wis => wis.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

