using Edgias.Agrik.API.Interfaces;
using Edgias.Agrik.API.Models.Form;
using Edgias.Agrik.API.Models.View;
using Edgias.Agrik.API.ObjectMappers;
using Edgias.Agrik.ApplicationCore.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Edgias.Agrik.API.Extensions
{
    public static class ObjectMapperExtension
    {
        public static void AddObjectMappers(this IServiceCollection services)
        {
            services.AddScoped<IMapper<CropCategory, CropCategoryRequestApiModel, CropCategoryApiModel>, CropCategoryMapper>();
            services.AddScoped<IMapper<CropUnit, CropUnitRequestApiModel, CropUnitApiModel>, CropUnitMapper>();
            services.AddScoped<IMapper<Crop, CropRequestApiModel, CropApiModel>, CropMapper>();
            services.AddScoped<IMapper<CropVariety, CropVarietyRequestApiModel, CropVarietyApiModel>, CropVarietyMapper>();
            services.AddScoped<IMapper<Field, FieldRequestApiModel, FieldApiModel>, FieldMapper>();
            services.AddScoped<IMapper<FieldMeasurement, FieldMeasurementRequestApiModel, FieldMeasurementApiModel>, FieldMeasurementMapper>();
            services.AddScoped<IMapper<Location, LocationRequestApiModel, LocationApiModel>, LocationMapper>();
            services.AddScoped<IMapper<OwnershipType, OwnershipTypeRequestApiModel, OwnershipTypeApiModel>, OwnershipTypeMapper>();
            services.AddScoped<IMapper<SeasonStatus, SeasonStatusRequestApiModel, SeasonStatusApiModel>, SeasonStatusMapper>();
            services.AddScoped<IMapper<Season, SeasonRequestApiModel, SeasonApiModel>, SeasonMapper>();
            services.AddScoped<IMapper<SoilType, SoilTypeRequestApiModel, SoilTypeApiModel>, SoilTypeMapper>();
            services.AddScoped<IMapper<WorkItemCategory, WorkItemCategoryRequestApiModel, WorkItemCategoryApiModel>, WorkItemCategoryMapper>();
            services.AddScoped<IMapper<WorkItemSubCategory, WorkItemSubCategoryRequestApiModel, WorkItemSubCategoryApiModel>, WorkItemSubCategoryMapper>();
            services.AddScoped<IMapper<WorkItemStatus, WorkItemStatusRequestApiModel, WorkItemStatusApiModel>, WorkItemStatusMapper>();
            services.AddScoped<IMapper<WorkItem, WorkItemRequestApiModel, WorkItemApiModel>, WorkItemMapper>();
        }
    }
}
