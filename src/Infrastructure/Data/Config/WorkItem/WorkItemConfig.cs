using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkItem;
using WorkItem = global::EdgIQ.MurimiOS.Domain.WorkItem.WorkItem;


internal class WorkItemConfig : BaseEntityConfig<WorkItem>
{
    public override void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        base.Configure(builder);

        builder.Property(wi => wi.Name)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(wi => wi.Description)
            .HasMaxLength(500);
    }
}


