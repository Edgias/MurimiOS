using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Season;
using Season = global::EdgIQ.MurimiOS.Domain.Season.Season;


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


