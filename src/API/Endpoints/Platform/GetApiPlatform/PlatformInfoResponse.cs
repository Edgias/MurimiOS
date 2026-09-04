namespace EdgIQ.MurimiOS.API.Endpoints.Platform.GetApiPlatform;

// Explicit transport contract for the previously anonymous protocol response.
public sealed record PlatformInfoResponse
{
    public string? name { get; init; }
    public string? apiVersion { get; init; }
    public int[]? syncSchemaVersions { get; init; }
    public string[]? syncCommands { get; init; }
}
