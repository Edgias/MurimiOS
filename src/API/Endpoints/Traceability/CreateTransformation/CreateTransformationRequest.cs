using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateTransformation;
public sealed record CreateTransformationRequest(Guid? Id, IReadOnlyList<TransformationInput> Inputs,
    IReadOnlyList<TransformationOutput> Outputs, decimal DeclaredLoss, string? LossUnit, DateTimeOffset OccurredAt,
    Guid? LocationId, string? BusinessStep, string? Disposition, string? TransactionReference, string? Notes);
