using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities.CropProductionAggregate;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class CropProductionConfig : BaseEntityConfig<CropProduction>
    {
        public override void Configure(EntityTypeBuilder<CropProduction> builder)
        {
            base.Configure(builder);

            builder.Property(cp => cp.Name)
                .HasMaxLength(180)
                .IsRequired();

            builder.Property(cp => cp.ExpectedYield)
                .HasColumnType("decimal(18,2)");

            builder.Property(cp => cp.ActualYield)
                .HasColumnType("decimal(18,2)");

            IMutableNavigation cropProductionFieldNavigation = builder.Metadata.FindNavigation(nameof(CropProduction.CropProductionFields));

            cropProductionFieldNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            IMutableNavigation cropProductionVarietyNavigation = builder.Metadata.FindNavigation(nameof(CropProduction.CropProductionVarieties));

            cropProductionVarietyNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
