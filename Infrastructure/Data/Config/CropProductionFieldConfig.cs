using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Murimi.ApplicationCore.Entities.CropProductionAggregate;

namespace Murimi.Infrastructure.Data.Config
{
    internal class CropProductionFieldConfig : BaseEntityConfig<CropProductionField>
    {
        public override void Configure(EntityTypeBuilder<CropProductionField> builder)
        {
            base.Configure(builder);

        }
    }
}
