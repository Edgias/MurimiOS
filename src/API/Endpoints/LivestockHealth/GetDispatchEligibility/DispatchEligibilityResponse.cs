using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.GetDispatchEligibility;
using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;

public sealed record DispatchEligibilityResponse(
    Guid SubjectId,
    LivestockDispatchPurpose Purpose,
    DateTimeOffset EvaluatedAt,
    bool Eligible,
    DateTimeOffset? BlockedUntil,
    IReadOnlyList<Guid> BlockingTreatmentIds);
