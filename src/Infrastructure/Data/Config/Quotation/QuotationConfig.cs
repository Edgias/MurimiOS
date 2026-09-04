using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Quotation;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;
using Quotation = global::EdgIQ.MurimiOS.Domain.Quotation.Quotation;


internal class QuotationConfig : BaseEntityConfig<Quotation>
{
    public override void Configure(EntityTypeBuilder<Quotation> builder)
    {
        base.Configure(builder);

        builder.Property(q => q.Name)
            .HasMaxLength(180)
            .IsRequired();

        IMutableNavigation? navigation = builder.Metadata.FindNavigation(nameof(Quotation.QuotationItems));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}


