using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.OwnershipType;
using OwnershipType = global::EdgIQ.MurimiOS.Domain.OwnershipType.OwnershipType;


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


