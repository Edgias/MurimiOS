using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

using EdgIQ.MurimiOS.Domain.Quotation;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Quotation;
using Quotation = global::EdgIQ.MurimiOS.Domain.Quotation.Quotation;

using ItemQuoted = global::EdgIQ.MurimiOS.Domain.Quotation.ItemQuoted;
using QuotationItem = global::EdgIQ.MurimiOS.Domain.Quotation.QuotationItem;

internal class QuotationItemConfig : BaseEntityConfig<QuotationItem>
{
    public override void Configure(EntityTypeBuilder<QuotationItem> builder)
    {
        base.Configure(builder);

        //builder.Property(pii => pii.UnitPrice)
        //    .HasColumnType("decimal(18,2)");

        builder.OwnsOne(qi => qi.ItemQuoted, iq =>
        {
            iq.WithOwner();

            iq.Property(i => i.ItemName)
                .HasMaxLength(160)
                .IsRequired();

            iq.Property(i => i.ItemDescription)
                .HasMaxLength(256)
                .IsRequired();
        });
    }
}


