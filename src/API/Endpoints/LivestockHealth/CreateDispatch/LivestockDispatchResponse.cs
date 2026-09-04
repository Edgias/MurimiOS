using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateDispatch;
using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;

public sealed record LivestockDispatchResponse(
    Guid Id,
    Guid SubjectId,
    LivestockDispatchPurpose Purpose,
    DateTimeOffset DispatchedAt,
    string Destination,
    DateTimeOffset? WithdrawalHoldUntil,
    string? OverrideReason,
    string? OverrideAuthorizedBy);
