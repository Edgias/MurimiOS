using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.RecordScreening;
public sealed record RecordEudrScreeningRequest(Guid? Id, string ProviderCode, string DatasetName,
    string DatasetVersion, DateOnly ObservationFrom, DateOnly ObservationThrough, decimal CoveragePercent,
    decimal PostCutoffTreeCoverLossHa, decimal DeforestationOverlapHa, decimal ConfidencePercent, string SourceReference,
    Guid? EvidenceAttachmentId, JsonElement ProviderResult);
