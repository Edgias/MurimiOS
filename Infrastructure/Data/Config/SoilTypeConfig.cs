using Edgias.MurimiOS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class SoilTypeConfig : BaseEntityConfig<SoilType>
    {
        public override void Configure(EntityTypeBuilder<SoilType> builder)
        {
            base.Configure(builder);

            builder.Property(st => st.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasIndex(st => st.Name)
                .IsUnique();
        }
    }
}
