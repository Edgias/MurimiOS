using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class NumberSequenceConfig : BaseEntityConfig<NumberSequence>
    {
        public override void Configure(EntityTypeBuilder<NumberSequence> builder)
        {
            base.Configure(builder);

            builder.Property(ns => ns.Entity)
                .HasMaxLength(160)
                .IsRequired();

            builder.HasIndex(ns => ns.Entity)
                .IsUnique();

            builder.Property(ns => ns.Prefix)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(ns => ns.Seperator)
                .HasMaxLength(1)
                .IsRequired();

            builder.Property(ns => ns.Suffix)
                .HasMaxLength(30);
        }
    }
}
