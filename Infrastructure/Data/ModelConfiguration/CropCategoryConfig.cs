using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.ModelConfiguration
{
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
}
