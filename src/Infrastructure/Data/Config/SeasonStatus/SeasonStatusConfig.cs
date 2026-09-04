using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SeasonStatus;
using SeasonStatus = global::EdgIQ.MurimiOS.Domain.SeasonStatus.SeasonStatus;


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


