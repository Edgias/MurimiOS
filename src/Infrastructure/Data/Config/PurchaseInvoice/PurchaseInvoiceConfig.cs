namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class PurchaseInvoiceConfig : BaseEntityConfig<PurchaseInvoice>
{
    public override void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
    {
        base.Configure(builder);

        builder.Property(pi => pi.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(pi => pi.InvoiceNotes)
            .HasMaxLength(500);

        builder.OwnsOne(pi => pi.BillingAddress, a =>
        {
            a.WithOwner();

            a.Property(a => a.Street)
                .HasMaxLength(180)
                .IsRequired();

            a.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            a.Property(a => a.State)
                .HasMaxLength(60)
                .IsRequired();

            a.Property(a => a.ZipCode)
                .HasMaxLength(18);

            a.Property(a => a.Country)
               .HasMaxLength(90)
               .IsRequired();
        });

        IMutableNavigation? navigation = builder.Metadata.FindNavigation(nameof(PurchaseInvoice.PurchaseInvoiceItems));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}


