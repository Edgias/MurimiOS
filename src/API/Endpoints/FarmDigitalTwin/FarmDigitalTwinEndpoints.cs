using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin;
using AddSoilSampleResultEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilSampleResult.AddSoilSampleResultEndpoint;
using AddSoilZoneBoundaryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilZoneBoundary.AddSoilZoneBoundaryEndpoint;
using ChangeInfrastructureStatusEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeInfrastructureStatus.ChangeInfrastructureStatusEndpoint;
using ChangeSoilSampleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeSoilSample.ChangeSoilSampleEndpoint;
using CloseTenureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CloseTenure.CloseTenureEndpoint;
using CreateInfrastructureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateInfrastructure.CreateInfrastructureEndpoint;
using CreateSoilSampleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilSample.CreateSoilSampleEndpoint;
using CreateSoilZoneEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilZone.CreateSoilZoneEndpoint;
using CreateTenureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateTenure.CreateTenureEndpoint;
using DecommissionInfrastructureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.DecommissionInfrastructure.DecommissionInfrastructureEndpoint;
using ListInfrastructureEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListInfrastructure.ListInfrastructureEndpoint;
using ListSoilSamplesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListSoilSamples.ListSoilSamplesEndpoint;
using ListSoilZonesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListSoilZones.ListSoilZonesEndpoint;
using ListTenuresEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListTenures.ListTenuresEndpoint;
using RetireSoilZoneEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.RetireSoilZone.RetireSoilZoneEndpoint;


public static class FarmDigitalTwinEndpoints
{
    public static void Map(RouteGroupBuilder api)
    {
        RouteGroupBuilder twin = api.MapGroup("/farms/{farmId:guid}/digital-twin").WithTags("Farm digital twin");
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Overview.OverviewEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListTenures.ListTenuresEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateTenure.CreateTenureEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CloseTenure.CloseTenureEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListInfrastructure.ListInfrastructureEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateInfrastructure.CreateInfrastructureEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeInfrastructureStatus.ChangeInfrastructureStatusEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.DecommissionInfrastructure.DecommissionInfrastructureEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListSoilZones.ListSoilZonesEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilZone.CreateSoilZoneEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilZoneBoundary.AddSoilZoneBoundaryEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.RetireSoilZone.RetireSoilZoneEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListSoilSamples.ListSoilSamplesEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilSample.CreateSoilSampleEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilSampleResult.AddSoilSampleResultEndpoint.Map(twin);
        global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeSoilSample.ChangeSoilSampleEndpoint.Map(twin);
    }
}
