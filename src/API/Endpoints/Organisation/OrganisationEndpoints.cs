
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Organisation;

public static class OrganisationEndpoints
{
    public static void MapOrganisationEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder organisations = api.MapGroup("/organisation/nodes").WithTags("Organisation");
        global::EdgIQ.MurimiOS.API.Endpoints.Organisation.List.ListEndpoint.Map(organisations);
        global::EdgIQ.MurimiOS.API.Endpoints.Organisation.Create.CreateEndpoint.Map(organisations);
    }
}
