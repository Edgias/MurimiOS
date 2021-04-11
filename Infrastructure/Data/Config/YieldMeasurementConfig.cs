using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Murimi.ApplicationCore.Entities;

namespace Murimi.Infrastructure.Data.Config
{
    internal class YieldMeasurementConfig : BaseEntityConfig<YieldMeasurement>
    {
        public override void Configure(EntityTypeBuilder<YieldMeasurement> builder)
        {
            base.Configure(builder);

            builder.Property(ym => ym.Name)
                .HasMaxLength(60)
                .IsRequired();
        }
    }
}
