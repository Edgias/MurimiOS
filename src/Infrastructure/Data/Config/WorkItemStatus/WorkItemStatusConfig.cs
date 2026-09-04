using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkItemStatus;
using WorkItemStatus = global::EdgIQ.MurimiOS.Domain.WorkItemStatus.WorkItemStatus;


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


