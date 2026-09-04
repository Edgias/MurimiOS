namespace Edgias.MurimiOS.Infrastructure.Data.Config;

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


