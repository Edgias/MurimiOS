using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction;
using ChangeGrazingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ChangeGrazing.ChangeGrazingEndpoint;
using ChangeRationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ChangeRation.ChangeRationEndpoint;
using CreateGrazingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateGrazing.CreateGrazingEndpoint;
using CreateMilkEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateMilk.CreateMilkEndpoint;
using CreateRationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation.CreateRationEndpoint;
using CreateReproductionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction.CreateReproductionEndpoint;
using FarmPerformanceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.FarmPerformance.FarmPerformanceEndpoint;
using GetProfileEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.GetProfile.GetProfileEndpoint;
using ListGrazingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListGrazing.ListGrazingEndpoint;
using ListMilkEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListMilk.ListMilkEndpoint;
using ListRationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListRations.ListRationsEndpoint;
using ListReproductionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListReproduction.ListReproductionEndpoint;
using PerformanceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.Performance.PerformanceEndpoint;


public static class LivestockProductionEndpoints
{
    public static void MapLivestockProductionEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder livestock = api.MapGroup("/livestock").WithTags("Livestock production");
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.GetProfile.GetProfileEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateProfile.CreateProfileEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListReproduction.ListReproductionEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateReproduction.CreateReproductionEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListMilk.ListMilkEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateMilk.CreateMilkEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListRations.ListRationsEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation.CreateRationEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ChangeRation.ChangeRationEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.Performance.PerformanceEndpoint.Map(livestock);

        RouteGroupBuilder farms = api.MapGroup("/farms/{farmId:guid}/livestock").WithTags("Livestock production");
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListGrazing.ListGrazingEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateGrazing.CreateGrazingEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ChangeGrazing.ChangeGrazingEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.FarmPerformance.FarmPerformanceEndpoint.Map(farms);
    }
}
