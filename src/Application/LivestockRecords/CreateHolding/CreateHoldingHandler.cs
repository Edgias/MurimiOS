using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateHolding;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;

public sealed class CreateHoldingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CreateHoldingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateHoldingCommand message, CancellationToken cancellationToken) => CreateHoldingAsync(message.farmId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CreateHoldingAsync(Guid farmId, CreateLivestockHoldingRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken) || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        Guid id = ValidId(request.Id);
        var holding = new LivestockHolding(id, farmId, request.Code, request.Name, request.Type);
        db.LivestockHoldings.Add(holding); await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/farms/{farmId}/livestock/holdings/{id}", new LivestockHoldingResponse(id, farmId, holding.Code, holding.Name, holding.Type));
    }

    private static Guid ValidId(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
