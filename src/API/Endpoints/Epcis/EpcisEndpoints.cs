using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Epcis;
using CaptureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Capture.CaptureEndpoint;
using CaptureStatusEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.CaptureStatus.CaptureStatusEndpoint;
using DiscoveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Discovery.DiscoveryEndpoint;
using ListCapturesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.ListCaptures.ListCapturesEndpoint;
using MappingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Mappings.MappingsEndpoint;
using QueryEventsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.QueryEvents.QueryEventsEndpoint;


public static class EpcisEndpoints
{

    public static void Map(RouteGroupBuilder api)
    {
        RouteGroupBuilder epcis = api.MapGroup("/epcis").WithTags("GS1 EPCIS 2.0.1");
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Capture.CaptureEndpoint.Map(epcis);
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.ListCaptures.ListCapturesEndpoint.Map(epcis);
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.CaptureStatus.CaptureStatusEndpoint.Map(epcis);
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.QueryEvents.QueryEventsEndpoint.Map(epcis);
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Mappings.MappingsEndpoint.Map(epcis);
        global::EdgIQ.MurimiOS.API.Endpoints.Epcis.Discovery.DiscoveryEndpoint.Map(epcis);
    }
}
