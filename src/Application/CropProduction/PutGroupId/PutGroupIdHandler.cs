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
using EdgIQ.MurimiOS.Domain.CropProduction;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropProduction.PutGroupId;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;

public sealed partial class PutGroupIdHandler(ILegacyDbContext dbContext) : IRequestHandler<PutGroupIdCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PutGroupIdCommand message, CancellationToken cancellationToken) => ExecuteAsync(dbContext, message.id, message.request);
private static async Task<UseCaseResult> ExecuteAsync(ILegacyDbContext dbContext,
            Guid id, CropProductionRequest request)         {
            if (id == default)
            {
                return UseCaseResults.BadRequest();
            }

            CropProduction? cropProduction = await dbContext.CropProductions.FindAsync(id);

            if (cropProduction is null)
            {
                return UseCaseResults.NotFound();
            }

            cropProduction.Update(request);

            dbContext.Update(cropProduction);
            await dbContext.SaveChangesAsync();

            return UseCaseResults.Ok();
        }

}
