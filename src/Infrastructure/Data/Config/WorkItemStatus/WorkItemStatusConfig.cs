namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class WorkItemStatusConfig : BaseEntityConfig<WorkItemStatus>
{
    public override void Configure(EntityTypeBuilder<WorkItemStatus> builder)
    {
        base.Configure(builder);

        builder.Property(wis => wis.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder.HasIndex(wis => wis.Name)
            .IsUnique();
    }
}


