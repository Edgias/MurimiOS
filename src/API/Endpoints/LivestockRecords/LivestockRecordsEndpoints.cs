using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords;
using CreateFeedAllocationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedAllocation.CreateFeedAllocationEndpoint;
using CreateFeedBatchEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedBatch.CreateFeedBatchEndpoint;
using CreateHealthEventEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHealthEvent.CreateHealthEventEndpoint;
using CreateHoldingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHolding.CreateHoldingEndpoint;
using CreateIdentifierEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateIdentifier.CreateIdentifierEndpoint;
using CreateMembershipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMembership.CreateMembershipEndpoint;
using CreateMovementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMovement.CreateMovementEndpoint;
using GetHistoryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.GetHistory.GetHistoryEndpoint;
using LeaveMembershipEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LeaveMembership.LeaveMembershipEndpoint;
using ListFeedAllocationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListFeedAllocations.ListFeedAllocationsEndpoint;
using ListFeedBatchesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListFeedBatches.ListFeedBatchesEndpoint;
using ListHealthEventsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListHealthEvents.ListHealthEventsEndpoint;
using ListHoldingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListHoldings.ListHoldingsEndpoint;
using ListIdentifiersEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListIdentifiers.ListIdentifiersEndpoint;
using ListMembershipsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListMemberships.ListMembershipsEndpoint;
using ListMovementsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListMovements.ListMovementsEndpoint;
using RetireIdentifierEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.RetireIdentifier.RetireIdentifierEndpoint;


public static class LivestockRecordsEndpoints
{
    public static void MapLivestockRecordsEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListHoldings.ListHoldingsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHolding.CreateHoldingEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListIdentifiers.ListIdentifiersEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateIdentifier.CreateIdentifierEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.RetireIdentifier.RetireIdentifierEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListMemberships.ListMembershipsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMembership.CreateMembershipEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LeaveMembership.LeaveMembershipEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListMovements.ListMovementsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMovement.CreateMovementEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListHealthEvents.ListHealthEventsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHealthEvent.CreateHealthEventEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListFeedBatches.ListFeedBatchesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedBatch.CreateFeedBatchEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListFeedAllocations.ListFeedAllocationsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedAllocation.CreateFeedAllocationEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.GetHistory.GetHistoryEndpoint.Map(api);
    }
}
