using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.GetUnits;
using UnitDimension = global::EdgIQ.MurimiOS.Domain.Common.UnitDimension;
using MeasurementUnitDefinition = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnitDefinition;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

public sealed class GetUnitsHandler() : IRequestHandler<GetUnitsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetUnitsQuery message, CancellationToken cancellationToken) => Task.FromResult(GetUnits(message.dimension));

    private static UseCaseResult GetUnits(UnitDimension? dimension)
    {
        IEnumerable<MeasurementUnitDefinition> units = MeasurementUnits.All;
        if (dimension is not null) units = units.Where(x => x.Dimension == dimension);
        return UseCaseResults.Ok(units.Select(x => new MeasurementUnitResponse(
            x.Code, x.Name, x.Symbol, x.Dimension, x.CanonicalCode, x.DefaultDecimalPlaces)));
    }


}
