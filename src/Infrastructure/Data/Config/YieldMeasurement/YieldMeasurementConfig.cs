using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.YieldMeasurement;
using YieldMeasurement = global::EdgIQ.MurimiOS.Domain.YieldMeasurement.YieldMeasurement;


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


