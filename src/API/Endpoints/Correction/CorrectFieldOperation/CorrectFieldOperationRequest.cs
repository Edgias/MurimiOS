using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectFieldOperation;

public sealed record CorrectFieldOperationRequest(
    Guid? CorrectionId,
    Guid? ReplacementId,
    Guid? CommandId,
    string OperationType,
    DateTimeOffset OccurredAt,
    string? Notes,
    string Reason);
