using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity;

public sealed record ConvertMeasurementRequest(decimal Value, string FromUnit, string ToUnit);
