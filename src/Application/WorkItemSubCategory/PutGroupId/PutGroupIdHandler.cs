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
using EdgIQ.MurimiOS.Domain.WorkItemSubCategory;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkItemSubCategory.PutGroupId;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;

public sealed partial class PutGroupIdHandler(ILegacyDbContext dbContext) : IRequestHandler<PutGroupIdCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PutGroupIdCommand message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.id, message.request);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext,
            Guid id, WorkItemSubCategoryRequest request)         {
            if (id == default)
            {
                return UseCaseResults.BadRequest();
            }

            WorkItemSubCategory? workItemSubCategory = await dbContext.WorkItemSubCategories.FindAsync(id);

            if (workItemSubCategory is null)
            {
                return UseCaseResults.NotFound();
            }

            workItemSubCategory.Update(request);

            dbContext.Update(workItemSubCategory);
            await dbContext.SaveChangesAsync();

            return UseCaseResults.Ok();
        }

}
