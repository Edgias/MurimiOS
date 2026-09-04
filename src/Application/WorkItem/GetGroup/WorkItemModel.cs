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
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkItem.GetGroup;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;
using WorkItem = global::EdgIQ.MurimiOS.Domain.WorkItem.WorkItem;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;
using WorkItemStatus = global::EdgIQ.MurimiOS.Domain.WorkItemStatus.WorkItemStatus;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;

public record WorkItemModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public Guid WorkItemStatusId { get; set; }

    [Required]
    public Guid WorkItemCategoryId { get; set; }

    public Guid? WorkItemSubCategoryId { get; set; }

    public Guid? FieldId { get; set; }

    [Required]
    public Guid SeasonId { get; set; }

    public Guid? CropProductionId { get; set; }

}
