using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockStatutory.CreateDocument;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using CropFeedSourceType = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedSourceType;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockRegulatoryExchangeType = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryExchangeType;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using StatutoryMovementDocumentStatus = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutoryMovementDocumentStatus;
using StatutorySubmissionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.StatutorySubmissionOutcome;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
public sealed record StatutoryMovementDocumentResponse(Guid Id, Guid MovementId, Guid SubjectId, Guid FarmId,
    Guid? RegulatoryProfileId, string Jurisdiction, string ProviderCode, string SchemaVersion, string DocumentNumber,
    string DocumentMediaType, JsonElement Snapshot,
    string PayloadSha256, StatutoryMovementDocumentStatus Status, DateTimeOffset CreatedAt, string CreatedBy,
    DateTimeOffset ReportingDueAt, DateTimeOffset RegisterDueAt, DateTimeOffset? SubmittedAt,
    DateTimeOffset? DecidedAt, string? ExternalReference, string? RejectionCode, string? RejectionMessage,
    IReadOnlyList<StatutorySubmissionAttemptResponse> Attempts);
