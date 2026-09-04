using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Evidence.InitiateUpload;

public sealed record EvidenceUploadResponse(
    Guid UploadId,
    Guid EvidenceId,
    long Offset,
    int ChunkSize,
    DateTimeOffset ExpiresAt,
    bool Completed);
