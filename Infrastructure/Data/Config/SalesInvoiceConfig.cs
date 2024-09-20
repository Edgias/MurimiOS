using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.SalesInvoiceAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class SalesInvoiceConfig : BaseEntityConfig<SalesInvoice>
    {
        public override void Configure(EntityTypeBuilder<SalesInvoice> builder)
        {
            base.Configure(builder);

            builder.Property(si => si.Name)
                .HasMaxLength(180)
                .IsRequired();

            builder.Property(si => si.InvoiceNotes)
                .HasMaxLength(500);

            builder.OwnsOne(si => si.BillingAddress, a =>
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

            IMutableNavigation navigation = builder.Metadata.FindNavigation(nameof(SalesInvoice.SalesInvoiceItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
