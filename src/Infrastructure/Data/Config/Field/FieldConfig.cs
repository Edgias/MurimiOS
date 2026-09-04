using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Field;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;


internal class FieldConfig : BaseEntityConfig<Field>
{
    public override void Configure(EntityTypeBuilder<Field> builder)
    {
        base.Configure(builder);

        builder.Property(f => f.Name)
            .HasMaxLength(256)
            .IsRequired();

        //builder.Property(f => f.UsableArea)
        //    .HasColumnType("decimal(18,2)");

        builder.HasIndex(f => f.Name)
            .IsUnique();
    }
}


