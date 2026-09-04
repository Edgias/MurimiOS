using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.RegionalFoundation.ConvertQuantity;
using UnitDimension = global::EdgIQ.MurimiOS.Domain.Common.UnitDimension;
using MeasurementUnitDefinition = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnitDefinition;
using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;
public sealed record ConvertedMeasurementResponse(
    decimal SourceValue,
    string SourceUnit,
    decimal ConvertedValue,
    string ConvertedUnit,
    UnitDimension Dimension,
    string CanonicalUnit,
    decimal CanonicalValue);
