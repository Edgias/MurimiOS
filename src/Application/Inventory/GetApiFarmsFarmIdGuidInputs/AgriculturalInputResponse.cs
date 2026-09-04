using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiFarmsFarmIdGuidInputs;


public sealed record AgriculturalInputResponse(Guid Id, Guid FarmId, string Name, string Category, string Unit, string DisplayUnit);
