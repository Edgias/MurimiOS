using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.WorkItemCategories;

internal static class WorkItemCategoryEndpoints
{
    public static RouteGroupBuilder MapWorkItemCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-categories");
        group.WithTags("Work Item Categories");
        group.WithParameterValidation(typeof(WorkItemCategoryRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<WorkItemCategory> workItemCategories = await dbContext.WorkItemCategories.ToListAsync();

            return workItemCategories.Select(wic => wic.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<WorkItemCategoryResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.WorkItemCategories.FindAsync(id) switch
            {
                WorkItemCategory workItemCategory when workItemCategory is not null => TypedResults.Ok(workItemCategory.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<WorkItemCategoryResponse>> (MurimiOSDbContext dbContext, 
            WorkItemCategoryRequest request) =>
        {
            WorkItemCategory workItemCategory = request.ToEntity();

            dbContext.Add(workItemCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/work-item-categories/{workItemCategory.Id}", workItemCategory.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, WorkItemCategoryRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            WorkItemCategory? workItemCategory = await dbContext.WorkItemCategories.FindAsync(id);

            if (workItemCategory is null)
            {
                return TypedResults.NotFound();
            }

            workItemCategory.Update(request);

            dbContext.Update(workItemCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.WorkItemCategories
            .Where(wic => wic.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

