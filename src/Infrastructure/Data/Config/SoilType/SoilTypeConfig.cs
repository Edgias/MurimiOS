using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilType;
using SoilType = global::EdgIQ.MurimiOS.Domain.SoilType.SoilType;


internal class SoilTypeConfig : BaseEntityConfig<SoilType>
{
    public override void Configure(EntityTypeBuilder<SoilType> builder)
    {
        base.Configure(builder);

        builder.Property(st => st.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(st => st.Name)
            .IsUnique();
    }
}


