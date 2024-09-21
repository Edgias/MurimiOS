using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.WorkItemSubCategories;

internal static class WorkItemSubCategoryEndpoints
{
    public static RouteGroupBuilder MapWorkItemSubCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-sub-categories");
        group.WithTags("Work Item Sub Categories");
        group.WithParameterValidation(typeof(WorkItemSubCategoryRequest));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<WorkItemSubCategory> workItemSubCategories = await dbContext.WorkItemSubCategories.ToListAsync();

            return workItemSubCategories.Select(wisc => wisc.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<WorkItemSubCategoryResponse>, NotFound>> 
            (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.WorkItemSubCategories.FindAsync(id) switch
            {
                WorkItemSubCategory workItemSubCategory when workItemSubCategory is not null => TypedResults.Ok(workItemSubCategory.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<WorkItemSubCategoryResponse>> (MurimiOSDbContext dbContext, 
            WorkItemSubCategoryRequest request) =>
        {
            WorkItemSubCategory workItemSubCategory = request.ToEntity();

            dbContext.Add(workItemSubCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/work-item-sub-categories/{workItemSubCategory.Id}", workItemSubCategory.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, WorkItemSubCategoryRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            WorkItemSubCategory? workItemSubCategory = await dbContext.WorkItemSubCategories.FindAsync(id);

            if (workItemSubCategory is null)
            {
                return TypedResults.NotFound();
            }

            workItemSubCategory.Update(request);

            dbContext.Update(workItemSubCategory);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.WorkItemSubCategories
            .Where(wisc => wisc.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }

}

