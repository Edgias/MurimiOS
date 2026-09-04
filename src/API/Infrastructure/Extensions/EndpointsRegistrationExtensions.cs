using EdgIQ.MurimiOS.API.Endpoints.Asset;
using EdgIQ.MurimiOS.API.Endpoints.Bins;
using EdgIQ.MurimiOS.API.Endpoints.BinType;
using EdgIQ.MurimiOS.API.Endpoints.CropCategory;
using EdgIQ.MurimiOS.API.Endpoints.CropProduction;
using EdgIQ.MurimiOS.API.Endpoints.Crop;
using EdgIQ.MurimiOS.API.Endpoints.CropUnit;
using EdgIQ.MurimiOS.API.Endpoints.CropVariety;
using EdgIQ.MurimiOS.API.Endpoints.Currency;
using EdgIQ.MurimiOS.API.Endpoints.Customer;
using EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory;
using EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement;
using EdgIQ.MurimiOS.API.Endpoints.Field;
using EdgIQ.MurimiOS.API.Endpoints.ItemCategory;
using EdgIQ.MurimiOS.API.Endpoints.Loan;
using EdgIQ.MurimiOS.API.Endpoints.Location;
using EdgIQ.MurimiOS.API.Endpoints.NumberSequence;
using EdgIQ.MurimiOS.API.Endpoints.OwnershipType;
using EdgIQ.MurimiOS.API.Endpoints.Season;
using EdgIQ.MurimiOS.API.Endpoints.SeasonStatus;
using EdgIQ.MurimiOS.API.Endpoints.SoilType;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory;
using EdgIQ.MurimiOS.API.Endpoints.WorkItem;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory;
using EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement;

namespace EdgIQ.MurimiOS.API.Infrastructure.Extensions
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
