namespace Edgias.MurimiOS.Infrastructure.Data.Config;

internal class EquipmentConfig : BaseEntityConfig<Equipment>
{
    public override void Configure(EntityTypeBuilder<Equipment> builder)
    {
        base.Configure(builder);

        builder.Property(m => m.Name)
            .HasMaxLength(90)
            .IsRequired();

        builder.Property(m => m.Manufacturer)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(m => m.Model)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(m => m.RegistrationNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(256);


    }
}


