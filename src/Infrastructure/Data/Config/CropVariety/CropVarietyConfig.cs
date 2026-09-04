namespace Edgias.MurimiOS.Infrastructure.Data.Config;

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


