using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProductionReport;
using GenerateEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.ProductionReport.Generate.GenerateEndpoint;


public static class ProductionReportEndpoints
{
    public static void MapProductionReportEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.ProductionReport.Generate.GenerateEndpoint.Map(api);
    }
}
