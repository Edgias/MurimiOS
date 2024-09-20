using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.SalesInvoiceAggregate;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class SalesInvoiceItemConfig : BaseEntityConfig<SalesInvoiceItem>
    {
        public override void Configure(EntityTypeBuilder<SalesInvoiceItem> builder)
        {
            base.Configure(builder);

            builder.Property(sii => sii.UnitPrice)
                .HasColumnType("decimal(18,2)");

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
}
