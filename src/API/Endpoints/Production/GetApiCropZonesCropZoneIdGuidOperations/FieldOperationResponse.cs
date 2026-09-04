using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Production.GetApiCropZonesCropZoneIdGuidOperations;

public sealed record FieldOperationResponse(
    Guid Id,
    Guid CropZoneId,
    string OperationType,
    DateTimeOffset OccurredAt,
    DateTimeOffset RecordedAt,
    string? Notes);
