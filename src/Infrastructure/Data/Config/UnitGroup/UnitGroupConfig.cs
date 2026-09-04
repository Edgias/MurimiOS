using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.UnitGroup;
using UnitGroup = global::EdgIQ.MurimiOS.Domain.UnitGroup.UnitGroup;


internal class UnitGroupConfig : BaseEntityConfig<UnitGroup>
{
    public override void Configure(EntityTypeBuilder<UnitGroup> builder)
    {
        base.Configure(builder);

        builder.Property(ug => ug.Name)
            .HasMaxLength(160)
            .IsRequired();
    }
}


