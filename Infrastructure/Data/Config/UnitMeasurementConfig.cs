using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
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
}
