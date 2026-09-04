using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.SalesOrder;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SalesOrder;
using SalesOrder = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrder;

using ItemOrdered = global::EdgIQ.MurimiOS.Domain.SalesOrder.ItemOrdered;
using SalesOrderItem = global::EdgIQ.MurimiOS.Domain.SalesOrder.SalesOrderItem;

internal class SalesOrderItemConfig : BaseEntityConfig<SalesOrderItem>
{
    public override void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        base.Configure(builder);

        //builder.Property(soi => soi.UnitPrice)
        //    .HasColumnType("decimal(18,2)");

        builder.OwnsOne(soi => soi.ItemOrdered, io =>
        {
            io.WithOwner();

            io.Property(i => i.ItemName)
                .HasMaxLength(160)
                .IsRequired();

            io.Property(i => i.ItemDescription)
                .HasMaxLength(256)
                .IsRequired();
        });
    }
}


