using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.ModelConfiguration
{
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
}
