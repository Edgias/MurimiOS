using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Assets;

public record AssetModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string SerialNumber { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? PurchaseInvoiceId { get; set; }

}

