namespace Edgias.MurimiOS.API.Assets;

public static class AssetMapper
{
    public static AssetResponse AsApiResponse(this Asset entity)
    {
        AssetResponse dto = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            SerialNumber = entity.SerialNumber,
            Description = entity.Description,
            PurchaseDate = entity.PurchaseDate,
            PurchaseInvoiceId = entity.PurchaseInvoiceId,
            PurchaseInvoiceNumber = entity.PurchaseInvoice?.Name!,
            PurchasePrice = entity.PurchasePrice,
            SupplierId = entity.SupplierId,
            SupplierName = entity.Supplier?.Name!
        };

        return dto;
    }

    public static Asset ToEntity(this AssetRequest request)
    {
        Asset entity = new(request.Name, request.SerialNumber, request.Description, request.PurchaseDate,
            request.PurchasePrice, request.SupplierId, request.PurchaseInvoiceId);

        return entity;
    }

    public static void Update(this Asset entity, AssetRequest request)
    {
        entity.Update(request.Name, request.SerialNumber, request.Description, request.PurchaseDate, request.PurchasePrice);
    }
}
