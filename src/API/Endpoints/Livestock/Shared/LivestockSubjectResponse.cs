using EdgIQ.MurimiOS.Domain.LivestockSubject;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Livestock.Shared;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;

public sealed record LivestockSubjectResponse(
    Guid Id,
    Guid FarmId,
    LivestockSubjectKind Kind,
    string Name,
    string Species,
    string? OfficialIdentifier);
