using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork;
using AcceptRelationshipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.AcceptRelationship.AcceptRelationshipEndpoint;
using CreateGrantEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateGrant.CreateGrantEndpoint;
using CreateRelationshipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateRelationship.CreateRelationshipEndpoint;
using ListAccessAuditEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListAccessAudit.ListAccessAuditEndpoint;
using ListGrantsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListGrants.ListGrantsEndpoint;
using ListRelationshipsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListRelationships.ListRelationshipsEndpoint;
using ReadSharedHarvestLotsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedHarvestLots.ReadSharedHarvestLotsEndpoint;
using ReadSharedTraceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedTrace.ReadSharedTraceEndpoint;
using RejectRelationshipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RejectRelationship.RejectRelationshipEndpoint;
using RevokeGrantEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RevokeGrant.RevokeGrantEndpoint;
using RevokeRelationshipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RevokeRelationship.RevokeRelationshipEndpoint;


public static class SupplyNetworkEndpoints
{
    public static void MapSupplyNetworkEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder supply = api.MapGroup("/supply-network").WithTags("Supply Network");
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListRelationships.ListRelationshipsEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateRelationship.CreateRelationshipEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.AcceptRelationship.AcceptRelationshipEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RejectRelationship.RejectRelationshipEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RevokeRelationship.RevokeRelationshipEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListGrants.ListGrantsEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateGrant.CreateGrantEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RevokeGrant.RevokeGrantEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListAccessAudit.ListAccessAuditEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedHarvestLots.ReadSharedHarvestLotsEndpoint.Map(supply);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedTrace.ReadSharedTraceEndpoint.Map(supply);
    }
}
