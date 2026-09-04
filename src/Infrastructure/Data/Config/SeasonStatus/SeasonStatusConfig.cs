namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class SeasonStatusConfig : BaseEntityConfig<SeasonStatus>
{
    public override void Configure(EntityTypeBuilder<SeasonStatus> builder)
    {
        base.Configure(builder);

        builder.Property(ss => ss.Name)
            .HasMaxLength(90)
            .IsRequired();

        builder.HasIndex(ss => ss.Name)
            .IsUnique();
    }
}


