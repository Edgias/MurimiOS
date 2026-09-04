using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.MurimiOS.Domain.Season;
using EdgIQ.MurimiOS.Domain.Crop;
using EdgIQ.MurimiOS.Domain.CropProduction;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropProduction.GetGroupId;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;

public static class CropProductionMapper
{
    public static CropProductionResponse AsApiResponse(this CropProduction entity)
    {
        CropProductionResponse response = new()
        {
            Id = entity.Id,
            ExpectedYield = entity.ExpectedYield,
            ActualYield = entity.ActualYield,
            Name = entity.Name,
            CropId = entity.CropId,
            CropName = entity.Crop?.Name!,
            SeasonId = entity.SeasonId,
            SeasonName = entity.Season?.Name!
        };

        return response;
    }

    public static CropProduction ToEntity(this CropProductionRequest request)
    {
        CropProduction entity = new(request.Name, request.CropId, request.SeasonId, request.ExpectedYield, request.ActualYield);

        return entity;
    }

    public static void Update(this CropProduction entity, CropProductionRequest request)
    {
        entity.UpdateDetails(request.Name, request.ExpectedYield, request.ActualYield);
    }
}
