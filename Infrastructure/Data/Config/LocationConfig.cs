using Edgias.MurimiOS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class LocationConfig : BaseEntityConfig<Location>
    {
        public override void Configure(EntityTypeBuilder<Location> builder)
        {
            base.Configure(builder);

            builder.Property(l => l.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(l => l.Name)
                .IsUnique();

            builder.OwnsOne(l => l.LocationAddress, a =>
            {
                a.WithOwner();

                a.Property(a => a.Street1)
                    .HasMaxLength(180)
                    .IsRequired();

                a.Property(a => a.Street2)
                    .HasMaxLength(180);

                a.Property(a => a.City)
                    .HasMaxLength(100)
                    .IsRequired();

                a.Property(a => a.State)
                    .HasMaxLength(60)
                    .IsRequired();

                a.Property(a => a.ZipCode)
                    .HasMaxLength(18);

                a.Property(a => a.Country)
                   .HasMaxLength(90)
                   .IsRequired();
            });

        }
    }
}
