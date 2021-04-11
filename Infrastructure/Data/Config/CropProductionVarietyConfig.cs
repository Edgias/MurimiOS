using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Murimi.ApplicationCore.Entities.CropProductionAggregate;

namespace Murimi.Infrastructure.Data.Config
{
    internal class CropProductionVarietyConfig : BaseEntityConfig<CropProductionVariety>
    {
        public override void Configure(EntityTypeBuilder<CropProductionVariety> builder)
        {
            base.Configure(builder);
        }
    }
}
