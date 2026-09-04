using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropUnit;
using CropUnit = global::EdgIQ.MurimiOS.Domain.CropUnit.CropUnit;


internal class CropUnitConfig : BaseEntityConfig<CropUnit>
{
    public override void Configure(EntityTypeBuilder<CropUnit> builder)
    {
        base.Configure(builder);

        builder.Property(cu => cu.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder.HasIndex(cu => cu.Name)
            .IsUnique();
    }
}


