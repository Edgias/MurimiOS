using Murimi.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Murimi.Infrastructure.Data.Config
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
