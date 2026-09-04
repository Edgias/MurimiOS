using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Farms.GetFarmsFarmIdGuidFields;


public sealed record FieldResponse(
    Guid Id,
    Guid FarmId,
    string Name,
    Guid BoundaryId,
    decimal AreaHa,
    DateOnly ValidFrom);
