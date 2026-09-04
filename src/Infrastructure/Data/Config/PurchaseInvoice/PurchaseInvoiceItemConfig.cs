using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.PurchaseInvoice;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PurchaseInvoice;
using PurchaseInvoice = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoice;

using InvoicedItem = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.InvoicedItem;
using PurchaseInvoiceItem = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoiceItem;

internal class PurchaseInvoiceItemConfig : BaseEntityConfig<PurchaseInvoiceItem>
{
    public override void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
    {
        base.Configure(builder);

        //builder.Property(pii => pii.UnitPrice)
        //    .HasColumnType("decimal(18,2)");

        builder.OwnsOne(pii => pii.InvoicedItem, ii =>
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


