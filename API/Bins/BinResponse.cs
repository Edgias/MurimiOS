namespace Edgias.MurimiOS.API.Bins
{
    public record BinResponse : BinModel
    {
        public Guid Id { get; set; }

        public string BinTypeName { get; set; } = default!;

        public string UnitMeasurementName { get; set; } = default!;

        public string WarehouseName { get; set; } = default!;
    }
}
