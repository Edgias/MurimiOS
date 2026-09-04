using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkItemSubCategory;
using WorkItemSubCategory = global::EdgIQ.MurimiOS.Domain.WorkItemSubCategory.WorkItemSubCategory;


internal class WorkItemSubCategoryConfig : BaseEntityConfig<WorkItemSubCategory>
{
    public override void Configure(EntityTypeBuilder<WorkItemSubCategory> builder)
    {
        base.Configure(builder);

        builder.Property(wisc => wisc.Name)
            .HasMaxLength(180)
            .IsRequired();
    }
}


