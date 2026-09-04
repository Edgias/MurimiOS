namespace EdgIQ.MurimiOS.API.Endpoints.Asset.Shared;

public record AssetResponse : AssetModel
{
    public Guid Id { get; set; }

    public string SupplierName { get; set; } = default!;

    public string PurchaseInvoiceNumber { get; set; } = default!;
}
