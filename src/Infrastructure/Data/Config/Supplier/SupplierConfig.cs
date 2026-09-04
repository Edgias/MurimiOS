using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Supplier;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;


internal class SupplierConfig : BaseEntityConfig<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
           .HasMaxLength(180)
           .IsRequired();

        builder.Property(s => s.Phone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(s => s.Website)
            .HasMaxLength(256);

        builder.Property(s => s.Email)
            .HasMaxLength(60);

        builder.Property(s => s.ContactPerson)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.ContactPersonEmail)
            .HasMaxLength(60);

        builder.Property(s => s.ContactPersonPhone)
            .HasMaxLength(30)
            .IsRequired();

        builder.OwnsOne(s => s.BillingAddress, a =>
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
    }
}


