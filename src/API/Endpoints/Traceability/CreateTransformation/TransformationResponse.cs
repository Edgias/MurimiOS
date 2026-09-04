using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
public sealed record TransformationResponse(Guid EventId, decimal DeclaredLoss, string Unit, IReadOnlyList<StorageLotResponse> Outputs);
