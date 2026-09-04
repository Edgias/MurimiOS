using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Evidence.GetApiCropZonesCropZoneIdGuidEvidence;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;

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
