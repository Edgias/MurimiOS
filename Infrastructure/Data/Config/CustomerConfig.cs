using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class CustomerConfig : BaseEntityConfig<Customer>
    {
        public override void Configure(EntityTypeBuilder<Customer> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name)
                .HasMaxLength(180)
                .IsRequired();

            builder.Property(c => c.Phone)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.Website)
                .HasMaxLength(256);

            builder.Property(c => c.Email)
                .HasMaxLength(60);

            builder.Property(c => c.ContactPerson)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(c => c.ContactPersonEmail)
                .HasMaxLength(60);

            builder.Property(c => c.ContactPersonPhone)
                .HasMaxLength(30)
                .IsRequired();

            builder.OwnsOne(c => c.BillingAddress, a =>
            {
                a.WithOwner();

                a.Property(a => a.Street1)
                    .HasMaxLength(180)
                    .IsRequired();

                a.Property(a => a.Street2)
                    .HasMaxLength(180);

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
}
