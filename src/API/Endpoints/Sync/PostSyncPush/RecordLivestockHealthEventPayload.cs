using EdgIQ.MurimiOS.Domain.LivestockSubject;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;

public sealed record RecordLivestockHealthEventPayload(
    Guid SubjectId,
    LivestockHealthEventType Type,
    string Title,
    DateTimeOffset OccurredAt,
    string? Diagnosis,
    string? Practitioner,
    string? Notes);
