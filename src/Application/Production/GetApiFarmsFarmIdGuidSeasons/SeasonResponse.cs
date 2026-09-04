using EdgIQ.MurimiOS.Domain.GrowingSeason;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.GetApiFarmsFarmIdGuidSeasons;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;

public sealed record SeasonResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    DateOnly StartsOn,
    DateOnly EndsOn,
    GrowingSeasonStatus Status);
