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
using EdgIQ.MurimiOS.Domain.WorkItem;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
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

public sealed partial class GetGroupHandler(ILegacyDbContext dbContext) : IRequestHandler<GetGroupQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetGroupQuery message, CancellationToken cancellationToken) => ExecuteAsync(dbContext);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext)         {
            List<WorkItem> workItems = await dbContext.WorkItems.ToListAsync();

            return UseCaseResults.Ok(workItems.Select(wi => wi.AsApiResponse()));
        }

}
