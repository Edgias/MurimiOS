namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class CurrencyConfig : BaseEntityConfig<Currency>
{
    public override void Configure(EntityTypeBuilder<Currency> builder)
    {
        base.Configure(builder);

        builder.Property(c => c.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(c => c.CurrencyCode)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(c => c.Symbol)
            .HasMaxLength(10)
            .IsRequired();
    }
}


