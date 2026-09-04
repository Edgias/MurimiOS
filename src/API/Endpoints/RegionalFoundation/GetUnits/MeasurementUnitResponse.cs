using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetUnits;

public sealed record MeasurementUnitResponse(
    string Code,
    string Name,
    string Symbol,
    UnitDimension Dimension,
    string CanonicalCode,
    int DefaultDecimalPlaces);
