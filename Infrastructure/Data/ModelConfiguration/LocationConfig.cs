using Edgias.Agrik.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.Agrik.Infrastructure.Data.ModelConfiguration
{
    internal class LocationConfig : BaseEntityConfig<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);

            builder.Property(l => l.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(l => l.ZipCode)
                .HasMaxLength(18);


            builder.Property(l => l.Street1)
                .HasMaxLength(180);

            builder.Property(l => l.Street2)
                .HasMaxLength(180);

            builder.Property(l => l.State)
                .HasMaxLength(60);

            builder.Property(l => l.Country)
                .HasMaxLength(90);

            builder.Property(l => l.City)
                .HasMaxLength(100);

            builder.HasIndex(l => l.Name)
                .IsUnique();
        }
    }
}
