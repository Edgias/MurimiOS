using Edgias.MurimiOS.API.Assets;
using Edgias.MurimiOS.API.Bins;
using Edgias.MurimiOS.API.BinTypes;
using Edgias.MurimiOS.API.CropCategories;
using Edgias.MurimiOS.API.CropProductions;
using Edgias.MurimiOS.API.Crops;
using Edgias.MurimiOS.API.CropUnits;
using Edgias.MurimiOS.API.CropVarieties;
using Edgias.MurimiOS.API.Currencies;
using Edgias.MurimiOS.API.Customers;
using Edgias.MurimiOS.API.EquipmentCategories;
using Edgias.MurimiOS.API.FieldMeasurements;
using Edgias.MurimiOS.API.Fields;
using Edgias.MurimiOS.API.ItemCategories;
using Edgias.MurimiOS.API.Loans;
using Edgias.MurimiOS.API.Locations;
using Edgias.MurimiOS.API.NumberSequences;
using Edgias.MurimiOS.API.OwnershipTypes;
using Edgias.MurimiOS.API.Seasons;
using Edgias.MurimiOS.API.SeasonStatuses;
using Edgias.MurimiOS.API.SoilTypes;
using Edgias.MurimiOS.API.WorkItemCategories;
using Edgias.MurimiOS.API.WorkItems;
using Edgias.MurimiOS.API.WorkItemStatuses;
using Edgias.MurimiOS.API.WorkItemSubCategories;
using Edgias.MurimiOS.API.YieldMeasurements;

namespace Edgias.MurimiOS.API.Extensions
{
    internal static class EndpointsRegistrationExtensions
    {
        public static void RegisterEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapAssets();
            builder.MapBins();
            builder.MapBinTypes();
            builder.MapCustomers();
            builder.MapCropCategories();
            builder.MapCropProductions();
            builder.MapCropUnits();
            builder.MapCropVarieties();
            builder.MapCrops();
            builder.MapCurrencies();
            builder.MapFieldMeasurements();
            builder.MapFields();
            builder.MapItemCategories();
            builder.MapLoans();
            builder.MapLocations();
            builder.MapMachineryCategories();
            builder.MapNumberSequences();
            builder.MapOwnershipTypes();
            builder.MapSeasons();
            builder.MapSeasonStatuses();
            builder.MapSoilTypes();
            builder.MapWorkItemCategories();
            builder.MapWorkItems();
            builder.MapWorkItemStatuses();
            builder.MapWorkItemSubCategories();
            builder.MapYieldMeasurements();
        }
    }
}
