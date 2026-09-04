using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport;
using ErrorsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Errors.ErrorsEndpoint;
using ExportEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Export.ExportEndpoint;
using TemplateEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Template.TemplateEndpoint;


public static class MasterDataImportEndpoints
{

    public static void Map(RouteGroupBuilder api)
    {
        RouteGroupBuilder imports = api.MapGroup("/imports/master-data").WithTags("Master-data imports");
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Template.TemplateEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.ListJobs.ListJobsEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.CreateJob.CreateJobEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.GetJob.GetJobEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Preview.PreviewEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Errors.ErrorsEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Apply.ApplyEndpoint.Map(imports);
        global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Export.ExportEndpoint.Map(imports);
    }
}
