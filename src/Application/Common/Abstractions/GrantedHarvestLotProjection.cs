
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class GrantedHarvestLotProjection
{
    public Guid GrantId { get; set; }
    public Guid ProducerTenantId { get; set; }
    public Guid HarvestLotId { get; set; }
    public Guid FarmId { get; set; }
    public Guid SeasonId { get; set; }
    public string? LotCode { get; set; }
    public string? Crop { get; set; }
    public string? Variety { get; set; }
    public decimal? Quantity { get; set; }
    public string? Unit { get; set; }
    public DateTimeOffset? HarvestedAt { get; set; }
}
