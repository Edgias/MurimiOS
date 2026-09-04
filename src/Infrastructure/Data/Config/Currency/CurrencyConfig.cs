using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Currency;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;


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


