using Edgias.MurimiOS.API.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Edgias.MurimiOS.API.Assets;

internal static class AssetEndpoints
{
    public static RouteGroupBuilder MapAssets(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/assets");
        group.WithTags("Assets");
        group.WithParameterValidation(typeof(AssetModel));

        group.MapGet("/", async (MurimiOSDbContext dbContext) =>
        {
            List<Asset> assets = await dbContext.Assets.ToListAsync();

            return assets.Select(a => a.AsApiResponse());
        });

        group.MapGet("/{id}", async Task<Results<Ok<AssetResponse>, NotFound>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            return await dbContext.Assets.FindAsync(id) switch
            {
                Asset asset when asset is not null => TypedResults.Ok(asset.AsApiResponse()),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<AssetResponse>> (MurimiOSDbContext dbContext, AssetRequest request) =>
        {
            Asset asset = request.ToEntity();

            dbContext.Add(asset);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"/assets/{asset.Id}", asset.AsApiResponse());
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (MurimiOSDbContext dbContext,
            Guid id, AssetRequest request) =>
        {
            if (id == default)
            {
                return TypedResults.BadRequest();
            }

            Asset? asset = await dbContext.Assets.FindAsync(id);

            if (asset is null)
            {
                return TypedResults.NotFound();
            }

            asset.Update(request);

            dbContext.Update(asset);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (MurimiOSDbContext dbContext, Guid id) =>
        {
            int rowsAffected = await dbContext.Assets
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();

        });

        return group;
    }

}

