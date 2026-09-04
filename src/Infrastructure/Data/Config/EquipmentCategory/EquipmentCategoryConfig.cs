using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EquipmentCategory;
using EquipmentCategory = global::EdgIQ.MurimiOS.Domain.EquipmentCategory.EquipmentCategory;


internal class EquipmentCategoryConfig : BaseEntityConfig<EquipmentCategory>
{
    public override void Configure(EntityTypeBuilder<EquipmentCategory> builder)
    {
        base.Configure(builder);

        builder.Property(mc => mc.Name)
            .HasMaxLength(160)
            .IsRequired();
    }
}


