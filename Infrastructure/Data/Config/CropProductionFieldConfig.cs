using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.CropProductionAggregate;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class CropProductionFieldConfig : BaseEntityConfig<CropProductionField>
    {
        public override void Configure(EntityTypeBuilder<CropProductionField> builder)
        {
            base.Configure(builder);

        }
    }
}
