using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.UnitMeasurement;
using UnitMeasurement = global::EdgIQ.MurimiOS.Domain.UnitMeasurement.UnitMeasurement;


internal class UnitMeasurementConfig : BaseEntityConfig<UnitMeasurement>
{
    public override void Configure(EntityTypeBuilder<UnitMeasurement> builder)
    {
        base.Configure(builder);

        builder.Property(um => um.Name)
            .HasMaxLength(180)
            .IsRequired();
    }
}


