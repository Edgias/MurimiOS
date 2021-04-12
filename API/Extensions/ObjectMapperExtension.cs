using Microsoft.Extensions.DependencyInjection;
using Murimi.API.Interfaces;
using Murimi.API.Models.Requests;
using Murimi.API.Models.Responses;
using Murimi.API.ObjectMappers;
using Murimi.ApplicationCore.Entities;

namespace Murimi.API.Extensions
{
    public static class ObjectMapperExtension
    {
        public static void AddObjectMappers(this IServiceCollection services)
        {
            services.AddScoped<IMapper<CropCategory, CropCategoryRequest, CropCategoryApiModel>, CropCategoryMapper>();
            services.AddScoped<IMapper<CropUnit, CropUnitRequest, CropUnitResponse>, CropUnitMapper>();
            services.AddScoped<IMapper<Crop, CropRequest, CropResponse>, CropMapper>();
            services.AddScoped<IMapper<CropVariety, CropVarietyRequest, CropVarietyResponse>, CropVarietyMapper>();
            services.AddScoped<IMapper<Field, FieldRequest, FieldResponse>, FieldMapper>();
            services.AddScoped<IMapper<FieldMeasurement, FieldMeasurementRequest, FieldMeasurementApiModel>, FieldMeasurementMapper>();
            services.AddScoped<IMapper<Location, LocationRequest, LocationResponse>, LocationMapper>();
            services.AddScoped<IMapper<OwnershipType, OwnershipTypeRequest, OwnershipTypeResponse>, OwnershipTypeMapper>();
            services.AddScoped<IMapper<SeasonStatus, SeasonStatusRequest, SeasonStatusResponse>, SeasonStatusMapper>();
            services.AddScoped<IMapper<Season, SeasonRequest, SeasonResponse>, SeasonMapper>();
            services.AddScoped<IMapper<SoilType, SoilTypeRequest, SoilTypeResponse>, SoilTypeMapper>();
            services.AddScoped<IMapper<WorkItemCategory, WorkItemCategoryRequest, WorkItemCategoryResponse>, WorkItemCategoryMapper>();
            services.AddScoped<IMapper<WorkItemSubCategory, WorkItemSubCategoryRequest, WorkItemSubCategoryResponse>, WorkItemSubCategoryMapper>();
            services.AddScoped<IMapper<WorkItemStatus, WorkItemStatusRequest, WorkItemStatusResponse>, WorkItemStatusMapper>();
            services.AddScoped<IMapper<WorkItem, WorkItemRequest, WorkItemResponse>, WorkItemMapper>();
        }
    }
}
