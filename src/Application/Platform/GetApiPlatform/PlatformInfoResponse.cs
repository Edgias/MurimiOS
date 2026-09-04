using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Platform.GetApiPlatform;


// Explicit transport contract for the previously anonymous protocol response.
public sealed record PlatformInfoResponse
{
    public string? name { get; init; }
    public string? apiVersion { get; init; }
    public int[]? syncSchemaVersions { get; init; }
    public string[]? syncCommands { get; init; }
}
