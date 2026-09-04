using EdgIQ.MurimiOS.Domain.GrowingSeason;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Production.PostApiFarmsFarmIdGuidSeasons;
using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;

public sealed record CreateSeasonRequest(
    Guid? Id,
    string Name,
    DateOnly StartsOn,
    DateOnly EndsOn,
    GrowingSeasonStatus Status);
