using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.ModelConfiguration
{
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
}
