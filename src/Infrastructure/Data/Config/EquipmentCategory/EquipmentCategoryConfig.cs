namespace Edgias.MurimiOS.Infrastructure.Data.Config;

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


