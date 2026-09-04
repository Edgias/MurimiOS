using EdgIQ.MurimiOS.Infrastructure.Data.Config.Common;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SalesInvoiceNote;
using SalesInvoiceNote = global::EdgIQ.MurimiOS.Domain.SalesInvoiceNote.SalesInvoiceNote;


internal class SalesInvoiceNoteConfig : BaseEntityConfig<SalesInvoiceNote>
{
    public override void Configure(EntityTypeBuilder<SalesInvoiceNote> builder)
    {
        base.Configure(builder);

        builder.Property(sin => sin.Description)
            .HasMaxLength(500)
            .IsRequired();
    }
}


