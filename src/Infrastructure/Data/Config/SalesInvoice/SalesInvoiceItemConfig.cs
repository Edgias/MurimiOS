using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.SalesInvoice;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SalesInvoice;
using SalesInvoice = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoice;

using InvoicedItem = global::EdgIQ.MurimiOS.Domain.SalesInvoice.InvoicedItem;
using SalesInvoiceItem = global::EdgIQ.MurimiOS.Domain.SalesInvoice.SalesInvoiceItem;

internal class SalesInvoiceItemConfig : BaseEntityConfig<SalesInvoiceItem>
{
    public override void Configure(EntityTypeBuilder<SalesInvoiceItem> builder)
    {
        base.Configure(builder);

        //builder.Property(sii => sii.UnitPrice)
        //    .HasColumnType("decimal(18,2)");

        builder.OwnsOne(sii => sii.InvoicedItem, ii =>
        {
            ii.WithOwner();

            ii.Property(i => i.ItemName)
                .HasMaxLength(160)
                .IsRequired();

            ii.Property(i => i.ItemDescription)
                .HasMaxLength(256)
                .IsRequired();
        });
    }
}


