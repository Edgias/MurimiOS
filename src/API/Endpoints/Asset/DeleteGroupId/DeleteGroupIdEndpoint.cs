using EdgIQ.MurimiOS.API.Infrastructure;
namespace EdgIQ.MurimiOS.API.Endpoints.Asset.DeleteGroupId;

internal static class DeleteGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapDelete("/{id}", async Task<IResult> (
            Dispatcher.ISender sender, 
            CancellationToken cancellationToken, Guid id) => (
            await sender.Send(new Application.Asset.DeleteGroupId.DeleteGroupIdCommand(id), cancellationToken))
            .ToHttpResult())
            .Produces(404)
            .Produces(200);

    }
}
