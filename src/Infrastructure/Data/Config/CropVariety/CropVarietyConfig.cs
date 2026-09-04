using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropVariety;
using CropVariety = global::EdgIQ.MurimiOS.Domain.CropVariety.CropVariety;


internal class CropVarietyConfig : BaseEntityConfig<CropVariety>
{
    public override void Configure(EntityTypeBuilder<CropVariety> builder)
    {
        base.Configure(builder);

        builder.Property(cv => cv.Name)
            .HasMaxLength(256)
            .IsRequired();

    }
}


