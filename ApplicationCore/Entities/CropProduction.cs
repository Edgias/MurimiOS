using System;

namespace Murimi.ApplicationCore.Entities
{
    public class CropProduction : BaseEntity
    {
        public string Name { get; set; }

        public Guid CropId { get; set; }

        public Crop Crop { get; set; }

        public Guid SeasonId { get; set; }

        public Season Season { get; set; }

        public decimal ExpectedYield { get; set; }

        public decimal? ActualYield { get; set; }
    }
}
