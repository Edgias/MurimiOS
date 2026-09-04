using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation;
using ConvertQuantityEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity.ConvertQuantityEndpoint;
using CreateCatalogueEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateCatalogue.CreateCatalogueEndpoint;
using CreateItemEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateItem.CreateItemEndpoint;
using GetCataloguesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetCatalogues.GetCataloguesEndpoint;
using GetItemsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetItems.GetItemsEndpoint;
using GetSettingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetSettings.GetSettingsEndpoint;
using GetUnitsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetUnits.GetUnitsEndpoint;
using PutSettingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.PutSettings.PutSettingsEndpoint;


public static class RegionalFoundationEndpoints
{
    public static void MapRegionalFoundationEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetSettings.GetSettingsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.PutSettings.PutSettingsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetUnits.GetUnitsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.ConvertQuantity.ConvertQuantityEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetCatalogues.GetCataloguesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateCatalogue.CreateCatalogueEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetItems.GetItemsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.CreateItem.CreateItemEndpoint.Map(api);
    }
}
