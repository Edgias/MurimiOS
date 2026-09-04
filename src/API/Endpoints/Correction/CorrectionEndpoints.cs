using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Correction;
using CorrectFieldOperationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectFieldOperation.CorrectFieldOperationEndpoint;
using CorrectLivestockMeasurementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectLivestockMeasurement.CorrectLivestockMeasurementEndpoint;

using ListEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Audit.List.ListEndpoint;

public static class CorrectionEndpoints
{
    public static void MapCorrectionEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectFieldOperation.CorrectFieldOperationEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectLivestockMeasurement.CorrectLivestockMeasurementEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Correction.List.ListEndpoint.Map(api);
    }
}
