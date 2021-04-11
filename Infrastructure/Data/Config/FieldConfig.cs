using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.Config
{
    internal class FieldConfig : BaseEntityConfig<Field>
    {
        public override void Configure(EntityTypeBuilder<Field> builder)
        {
            base.Configure(builder);

            builder.Property(f => f.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(f => f.UsableArea)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(f => f.Name)
                .IsUnique();
        }
    }
}
