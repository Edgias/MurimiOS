using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.PostApiFarmsFarmIdGuidInputs;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;

public sealed partial class PostApiFarmsFarmIdGuidInputsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<PostApiFarmsFarmIdGuidInputsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostApiFarmsFarmIdGuidInputsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.farmId, message.request, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid farmId,
            CreateAgriculturalInputRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)
                || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken))
            {
                return UseCaseResults.NotFound();
            }

            try
            {
                var input = new AgriculturalInput(Guid.NewGuid(), farmId, request.Name, request.Category, request.Unit);
                db.AgriculturalInputs.Add(input);
                await db.SaveChangesAsync(cancellationToken);
                return UseCaseResults.Created(
                    $"/api/v1/farms/{farmId}/inputs/{input.Id}",
                    new AgriculturalInputResponse(input.Id, farmId, input.Name, input.Category, input.Unit, input.DisplayUnit));
            }
            catch (ArgumentException exception)
            {
                return UseCaseResults.BadRequest(new { code = "agricultural_input_invalid", message = exception.Message });
            }
        }

}
