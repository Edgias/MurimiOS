using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.NumberSequence;
using NumberSequence = global::EdgIQ.MurimiOS.Domain.NumberSequence.NumberSequence;


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


