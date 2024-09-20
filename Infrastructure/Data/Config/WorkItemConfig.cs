using Edgias.MurimiOS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
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
