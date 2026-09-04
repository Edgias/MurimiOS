using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Audit;

public static class AuditEndpoints
{
    public static void MapAuditEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Audit.List.ListEndpoint.Map(api);
    }
}
