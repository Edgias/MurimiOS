using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.PurchaseInvoiceAggregate;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class PurchaseInvoiceItemConfig : BaseEntityConfig<PurchaseInvoiceItem>
    {
        public override void Configure(EntityTypeBuilder<PurchaseInvoiceItem> builder)
        {
            base.Configure(builder);

            builder.Property(pii => pii.UnitPrice)
                .HasColumnType("decimal(18,2)");

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
}
