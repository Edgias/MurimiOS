using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.RecordScreening;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
public sealed record RecordEudrScreeningRequest(Guid? Id, string ProviderCode, string DatasetName,
    string DatasetVersion, DateOnly ObservationFrom, DateOnly ObservationThrough, decimal CoveragePercent,
    decimal PostCutoffTreeCoverLossHa, decimal DeforestationOverlapHa, decimal ConfidencePercent, string SourceReference,
    Guid? EvidenceAttachmentId, JsonElement ProviderResult);
