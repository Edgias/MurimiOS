using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
public sealed record StorageLotResponse(Guid Id, Guid FarmId, string LotCode, string Crop, string? Variety,
    decimal Quantity, string Unit, decimal EnteredQuantity, string EnteredUnit, DateTimeOffset PackedAt, string? StorageLocation);
