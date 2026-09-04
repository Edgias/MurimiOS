using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarms;

public sealed record CreateFarmRequest(
    Guid? Id,
    string Name,
    string Code,
    string CountryCode,
    string TimeZone,
    Guid? ParentOrganisationNodeId = null);
