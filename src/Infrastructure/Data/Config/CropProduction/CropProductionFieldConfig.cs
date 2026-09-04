using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.CropProduction;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropProduction;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;

using CropProductionField = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProductionField;

internal class CropProductionFieldConfig : BaseEntityConfig<CropProductionField>
{
    public override void Configure(EntityTypeBuilder<CropProductionField> builder)
    {
        base.Configure(builder);

    }
}


