using EdgIQ.MurimiOS.Domain.GrowingSeason;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Production.PostApiFarmsFarmIdGuidSeasons;
using GrowingSeasonStatus = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeasonStatus;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;

public sealed record CreateSeasonRequest(
    Guid? Id,
    string Name,
    DateOnly StartsOn,
    DateOnly EndsOn,
    GrowingSeasonStatus Status);
