using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Asset;
using Asset = global::EdgIQ.MurimiOS.Domain.Asset.Asset;


internal class AssetConfig : BaseEntityConfig<Asset>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.Property(a => a.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(a => a.SerialNumber)
            .HasMaxLength(90)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.Property(a => a.PurchasePrice)
            .HasColumnType("decimal(18,2)");
    }
}


