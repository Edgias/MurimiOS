namespace Edgias.MurimiOS.API.Assets;

public record AssetResponse : AssetModel
{
    public Guid Id { get; set; }

    public string SupplierName { get; set; } = default!;

    public string PurchaseInvoiceNumber { get; set; } = default!;
}
