using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Crop;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;


internal class CropConfig : BaseEntityConfig<Crop>
{
    public override void Configure(EntityTypeBuilder<Crop> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(c => c.Name)
            .IsUnique();
    }
}


