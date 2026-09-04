using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldMeasurement;
using FieldMeasurement = global::EdgIQ.MurimiOS.Domain.FieldMeasurement.FieldMeasurement;


internal class FieldMeasurementConfig : BaseEntityConfig<FieldMeasurement>
{
    public override void Configure(EntityTypeBuilder<FieldMeasurement> builder)
    {
        base.Configure(builder);

        builder.Property(fm => fm.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(fm => fm.Name)
            .IsUnique();
    }
}


