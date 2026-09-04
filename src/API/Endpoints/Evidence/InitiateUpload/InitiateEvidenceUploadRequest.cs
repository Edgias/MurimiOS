using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.InitiateUpload;

public sealed record InitiateEvidenceUploadRequest(
    Guid EvidenceId,
    Guid CropZoneId,
    string FileName,
    string ContentType,
    long ByteSize,
    string Sha256,
    DateTimeOffset CapturedAt,
    double? Latitude,
    double? Longitude,
    double? AccuracyMetres);
