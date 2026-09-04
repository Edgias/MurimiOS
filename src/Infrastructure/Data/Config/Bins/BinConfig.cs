using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Bins;
using Bin = global::EdgIQ.MurimiOS.Domain.Bins.Bin;


internal class BinConfig : BaseEntityConfig<Bin>
{
    public override void Configure(EntityTypeBuilder<Bin> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Name)
            .HasMaxLength(180)
            .IsRequired();

        //builder.Property(b => b.Capacity)
        //    .HasColumnType("decimal(18,2)");
    }
}


