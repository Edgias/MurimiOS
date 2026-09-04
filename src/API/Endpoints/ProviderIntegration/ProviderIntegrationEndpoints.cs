using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration;
using ActivateConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ActivateConnection.ActivateConnectionEndpoint;
using CancelJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CancelJob.CancelJobEndpoint;
using ClaimJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ClaimJob.ClaimJobEndpoint;
using CompleteJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CompleteJob.CompleteJobEndpoint;
using CreateConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateConnection.CreateConnectionEndpoint;
using CreateFeedEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateFeed.CreateFeedEndpoint;
using DisableConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.DisableConnection.DisableConnectionEndpoint;
using DisableFeedEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.DisableFeed.DisableFeedEndpoint;
using EnqueueJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.EnqueueJob.EnqueueJobEndpoint;
using FailJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.FailJob.FailJobEndpoint;
using GetConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetConnection.GetConnectionEndpoint;
using GetRecordEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetRecord.GetRecordEndpoint;
using ListConnectionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListConnections.ListConnectionsEndpoint;
using ListFeedsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListFeeds.ListFeedsEndpoint;
using PauseConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.PauseConnection.PauseConnectionEndpoint;
using PauseFeedEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.PauseFeed.PauseFeedEndpoint;
using ProcessRecordEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProcessRecord.ProcessRecordEndpoint;
using ReceiveRecordsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords.ReceiveRecordsEndpoint;
using RejectRecordEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RejectRecord.RejectRecordEndpoint;
using ReplayJobEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReplayJob.ReplayJobEndpoint;
using ResumeFeedEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ResumeFeed.ResumeFeedEndpoint;
using RevokeCredentialEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RevokeCredential.RevokeCredentialEndpoint;
using RotateCredentialEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RotateCredential.RotateCredentialEndpoint;
using UpdateConnectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.UpdateConnection.UpdateConnectionEndpoint;


public static class ProviderIntegrationEndpoints
{

    public static void MapProviderIntegrationEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder integrations = api.MapGroup("/integrations").WithTags("Provider Integrations");
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListConnections.ListConnectionsEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateConnection.CreateConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetConnection.GetConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.UpdateConnection.UpdateConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RotateCredential.RotateCredentialEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RevokeCredential.RevokeCredentialEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ActivateConnection.ActivateConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.PauseConnection.PauseConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.DisableConnection.DisableConnectionEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListFeeds.ListFeedsEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateFeed.CreateFeedEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.PauseFeed.PauseFeedEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ResumeFeed.ResumeFeedEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.DisableFeed.DisableFeedEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListJobs.ListJobsEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.EnqueueJob.EnqueueJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetJob.GetJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ClaimJob.ClaimJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords.ReceiveRecordsEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CompleteJob.CompleteJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.FailJob.FailJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReplayJob.ReplayJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CancelJob.CancelJobEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetRecord.GetRecordEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProcessRecord.ProcessRecordEndpoint.Map(integrations);
        global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.RejectRecord.RejectRecordEndpoint.Map(integrations);
    }
}
