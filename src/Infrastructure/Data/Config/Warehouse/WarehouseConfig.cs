using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Warehouse;
using Warehouse = global::EdgIQ.MurimiOS.Domain.Warehouse.Warehouse;


internal class WarehouseConfig : BaseEntityConfig<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.Name)
            .HasMaxLength(180)
            .IsRequired();
    }
}


