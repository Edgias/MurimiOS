﻿namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class SeasonConfig : BaseEntityConfig<Season>
{
    public override void Configure(EntityTypeBuilder<Season> builder)
    {
        base.Configure(builder);

        builder.Property(s => s.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(s => s.Name)
            .IsUnique();
    }
}


