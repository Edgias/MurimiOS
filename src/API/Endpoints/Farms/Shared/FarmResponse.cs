using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Farms.Shared;

public sealed record FarmResponse(
    Guid Id,
    Guid OrganisationNodeId,
    string Name,
    string Code,
    string CountryCode,
    string TimeZone);
