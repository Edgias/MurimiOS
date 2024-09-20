using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class BinConfig : BaseEntityConfig<Bin>
    {
        public override void Configure(EntityTypeBuilder<Bin> builder)
        {
            base.Configure(builder);

            builder.Property(b => b.Name)
                .HasMaxLength(180)
                .IsRequired();

            builder.Property(b => b.Capacity)
                .HasColumnType("decimal(18,2)");
        }
    }
}
