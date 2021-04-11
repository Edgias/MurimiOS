using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Murimi.ApplicationCore.Entities.SalesOrderAggregate;

namespace Murimi.Infrastructure.Data.Config
{
    internal class SalesOrderItemConfig : BaseEntityConfig<SalesOrderItem>
    {
        public override void Configure(EntityTypeBuilder<SalesOrderItem> builder)
        {
            base.Configure(builder);

            builder.Property(soi => soi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.OwnsOne(soi => soi.ItemOrdered, io =>
            {
                io.WithOwner();

                io.Property(i => i.ItemName)
                    .HasMaxLength(160)
                    .IsRequired();

                io.Property(i => i.ItemDescription)
                    .HasMaxLength(256)
                    .IsRequired();
            });
        }
    }
}
