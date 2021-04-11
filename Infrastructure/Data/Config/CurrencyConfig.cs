using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Murimi.ApplicationCore.Entities;

namespace Murimi.Infrastructure.Data.Config
{
    internal class CurrencyConfig : BaseEntityConfig<Currency>
    {
        public override void Configure(EntityTypeBuilder<Currency> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(c => c.Symbol)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}
