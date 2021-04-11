using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.Config
{
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
}
