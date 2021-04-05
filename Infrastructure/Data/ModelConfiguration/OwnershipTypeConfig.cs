using Edgias.Agrik.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edgias.Agrik.Infrastructure.Data.ModelConfiguration
{
    internal class OwnershipTypeConfig : BaseEntityConfig<OwnershipType>
    {
        public override void Configure(EntityTypeBuilder<OwnershipType> builder)
        {
            base.Configure(builder);

            builder.Property(ot => ot.Name)
                .HasMaxLength(90)
                .IsRequired();

            builder.HasIndex(ot => ot.Name)
                .IsUnique();
        }
    }
}
