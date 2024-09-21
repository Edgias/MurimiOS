﻿namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class CropUnitConfig : BaseEntityConfig<CropUnit>
{
    public override void Configure(EntityTypeBuilder<CropUnit> builder)
    {
        base.Configure(builder);

        builder.Property(cu => cu.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder.HasIndex(cu => cu.Name)
            .IsUnique();
    }
}


