using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.Shared;

public sealed record EvidenceResponse(
    Guid Id,
    Guid CropZoneId,
    string FileName,
    string ContentType,
    long ByteSize,
    string Sha256,
    DateTimeOffset CapturedAt,
    double? Latitude,
    double? Longitude,
    double? AccuracyMetres,
    string ContentUrl);
