using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Infrastructure.Data.Config
{
    internal class MachineryCategoryConfig : BaseEntityConfig<MachineryCategory>
    {
        public override void Configure(EntityTypeBuilder<MachineryCategory> builder)
        {
            base.Configure(builder);

            builder.Property(mc => mc.Name)
                .HasMaxLength(160)
                .IsRequired();
        }
    }
}
