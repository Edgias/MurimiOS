using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
public sealed record TransformationOutput(Guid? Id, string LotCode, decimal Quantity, string Unit, string? StorageLocation);
