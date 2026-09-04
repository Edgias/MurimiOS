using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropCategory;
using CropCategory = global::EdgIQ.MurimiOS.Domain.CropCategory.CropCategory;


internal class CropCategoryConfig : BaseEntityConfig<CropCategory>
{
    public override void Configure(EntityTypeBuilder<CropCategory> builder)
    {
        base.Configure(builder);

        builder.Property(cc => cc.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(cc => cc.Description)
            .HasMaxLength(500);

        builder.HasIndex(cc => cc.Name)
            .IsUnique();
    }
}


