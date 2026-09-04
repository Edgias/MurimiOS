using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkItemCategory;
using WorkItemCategory = global::EdgIQ.MurimiOS.Domain.WorkItemCategory.WorkItemCategory;


internal class WorkItemCategoryConfig : BaseEntityConfig<WorkItemCategory>
{
    public override void Configure(EntityTypeBuilder<WorkItemCategory> builder)
    {
        base.Configure(builder);

        builder.Property(wic => wic.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasIndex(wic => wic.Name)
            .IsUnique();
    }
}


