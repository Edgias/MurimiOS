using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class SalesOrderConfig : BaseEntityConfig<SalesOrder>
    {
        public override void Configure(EntityTypeBuilder<SalesOrder> builder)
        {
            base.Configure(builder);

            builder.Property(so => so.Name)
                .HasMaxLength(180)
                .IsRequired();

            builder.OwnsOne(so => so.ShipToAddress, a =>
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

            IMutableNavigation navigation = builder.Metadata.FindNavigation(nameof(SalesOrder.SalesOrderItems));

            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
