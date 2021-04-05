using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.ModelConfiguration
{
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
}
