using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectLivestockMeasurement;

public sealed record CorrectLivestockMeasurementRequest(
    Guid? CorrectionId,
    Guid? ReplacementId,
    Guid? CommandId,
    string Metric,
    decimal Value,
    string Unit,
    DateTimeOffset OccurredAt,
    string Reason);
