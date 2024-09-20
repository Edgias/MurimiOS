using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class MachineryConfig : BaseEntityConfig<Machinery>
    {
        public override void Configure(EntityTypeBuilder<Machinery> builder)
        {
            base.Configure(builder);

            builder.Property(m => m.Name)
                .HasMaxLength(90)
                .IsRequired();

            builder.Property(m => m.Manufacturer)
                .HasMaxLength(160)
                .IsRequired();

            builder.Property(m => m.Model)
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(m => m.RegistrationNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(256);


        }
    }
}
