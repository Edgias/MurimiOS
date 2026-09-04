using EdgIQ.MurimiOS.Domain.GrowingSeason;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Production.Shared;
using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;

public sealed record SeasonResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    DateOnly StartsOn,
    DateOnly EndsOn,
    GrowingSeasonStatus Status);
