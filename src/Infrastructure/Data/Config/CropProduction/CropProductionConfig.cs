using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropProduction;
using CropProduction = global::EdgIQ.MurimiOS.Domain.CropProduction.CropProduction;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;


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

        IMutableNavigation? cropProductionFieldNavigation = builder.Metadata.FindNavigation(nameof(CropProduction.CropProductionFields));
        cropProductionFieldNavigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        IMutableNavigation? cropProductionVarietyNavigation = builder.Metadata.FindNavigation(nameof(CropProduction.CropProductionVarieties));
        cropProductionVarietyNavigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}


