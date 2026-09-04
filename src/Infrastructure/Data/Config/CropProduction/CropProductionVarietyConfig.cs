using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.CropProduction;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropProduction;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;

using CropProductionVariety = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProductionVariety;

internal class CropProductionVarietyConfig : BaseEntityConfig<CropProductionVariety>
{
    public override void Configure(EntityTypeBuilder<CropProductionVariety> builder)
    {
        base.Configure(builder);
    }
}


