using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PriceList;
using PriceList = global::EdgIQ.MurimiOS.Domain.PriceList.PriceList;


internal class PriceListConfig : BaseEntityConfig<PriceList>
{
    public override void Configure(EntityTypeBuilder<PriceList> builder)
    {
        base.Configure(builder);

        builder.Property(pl => pl.Name)
            .HasMaxLength(180)
            .IsRequired();

        //builder.Property(pl => pl.UnitPrice)
        //    .HasColumnType("decimal(18,2)");
    }
}


