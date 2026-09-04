using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity;
public sealed record ConvertedMeasurementResponse(
    decimal SourceValue,
    string SourceUnit,
    decimal ConvertedValue,
    string ConvertedUnit,
    UnitDimension Dimension,
    string CanonicalUnit,
    decimal CanonicalValue);
