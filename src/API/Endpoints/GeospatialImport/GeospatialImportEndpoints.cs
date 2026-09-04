using System.Security.Cryptography;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport;

public static class GeospatialImportEndpoints
{

    public static void MapGeospatialImportEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder imports = api.MapGroup("/imports/geospatial").WithTags("Geospatial imports");
        global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.ListJobs.ListJobsEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.CreateJob.CreateJobEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GetJob.GetJobEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Preview.PreviewEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply.ApplyEndpoint.Map(imports);
    }
}
