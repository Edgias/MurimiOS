using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.ConvertQuantity;
using UnitDimension = global::EdgIQ.MurimiOS.Domain.Common.UnitDimension;
using MeasurementUnitDefinition = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnitDefinition;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

public sealed class ConvertQuantityHandler() : IRequestHandler<ConvertQuantityCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ConvertQuantityCommand message, CancellationToken cancellationToken) => Task.FromResult(ConvertQuantity(message.request));

    private static UseCaseResult ConvertQuantity(ConvertMeasurementRequest request)
    {
        try
        {
            MeasurementUnitDefinition source = MeasurementUnits.Get(request.FromUnit);
            MeasurementUnitDefinition target = MeasurementUnits.Get(request.ToUnit);
            decimal converted = MeasurementUnits.Convert(request.Value, source.Code, target.Code);
            return UseCaseResults.Ok(new ConvertedMeasurementResponse(
                request.Value,
                source.Code,
                converted,
                target.Code,
                source.Dimension,
                source.CanonicalCode,
                MeasurementUnits.Convert(request.Value, source.Code, source.CanonicalCode)));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "unit_conversion_invalid", message = exception.Message });
        }
    }


}
