
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Evidence;
using AppendChunkEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Evidence.AppendChunk.AppendChunkEndpoint;
using CompleteUploadEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Evidence.CompleteUpload.CompleteUploadEndpoint;
using GetApiCropZonesCropZoneIdGuidEvidenceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Evidence.GetApiCropZonesCropZoneIdGuidEvidence.GetApiCropZonesCropZoneIdGuidEvidenceEndpoint;
using InitiateUploadEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Evidence.InitiateUpload.InitiateUploadEndpoint;


public static class EvidenceEndpoints
{

    public static void MapEvidenceEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Evidence.InitiateUpload.InitiateUploadEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Evidence.AppendChunk.AppendChunkEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Evidence.CompleteUpload.CompleteUploadEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Evidence.GetApiCropZonesCropZoneIdGuidEvidence.GetApiCropZonesCropZoneIdGuidEvidenceEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Evidence.Download.DownloadEndpoint.Map(api);
    }
}
