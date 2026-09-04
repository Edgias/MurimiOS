using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Tax;
using Tax = global::EdgIQ.MurimiOS.Domain.Tax.Tax;


internal class TaxConfig : BaseEntityConfig<Tax>
{
    public override void Configure(EntityTypeBuilder<Tax> builder)
    {
        base.Configure(builder);

        builder.Property(t => t.Name)
            .HasMaxLength(90)
            .IsRequired();

        //builder.Property(t => t.Percentage)
        //    .HasColumnType("decimal(18,2)");
    }
}


