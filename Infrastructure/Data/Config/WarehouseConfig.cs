namespace Edgias.MurimiOS.Infrastructure.Data.Config;

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


