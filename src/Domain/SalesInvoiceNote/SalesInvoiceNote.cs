namespace EdgIQ.MurimiOS.Domain.SalesInvoiceNote;

public class SalesInvoiceNote(string description) : BaseEntity, IAggregateRoot
{
    public string Description { get; private set; } = description;

    public void Update(string description)
    {
        Description = description;
    }

}


