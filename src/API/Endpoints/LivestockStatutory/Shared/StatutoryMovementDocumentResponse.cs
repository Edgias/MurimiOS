using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.Shared;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
public sealed record StatutoryMovementDocumentResponse(Guid Id, Guid MovementId, Guid SubjectId, Guid FarmId,
    Guid? RegulatoryProfileId, string Jurisdiction, string ProviderCode, string SchemaVersion, string DocumentNumber,
    string DocumentMediaType, JsonElement Snapshot,
    string PayloadSha256, StatutoryMovementDocumentStatus Status, DateTimeOffset CreatedAt, string CreatedBy,
    DateTimeOffset ReportingDueAt, DateTimeOffset RegisterDueAt, DateTimeOffset? SubmittedAt,
    DateTimeOffset? DecidedAt, string? ExternalReference, string? RejectionCode, string? RejectionMessage,
    IReadOnlyList<StatutorySubmissionAttemptResponse> Attempts);
