using Edgias.MurimiOS.Domain.SharedKernel;
using System;
using System.Collections.Generic;

namespace Edgias.MurimiOS.Domain.Entities.CropProductionAggregate
{
    public class CropProduction : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }

        public Guid CropId { get; private set; }

        public Crop Crop { get; private set; }

        public Guid SeasonId { get; private set; }

        public Season Season { get; private set; }

        public decimal ExpectedYield { get; private set; }

        public decimal? ActualYield { get; private set; }

        private readonly List<CropProductionField> _cropProductionFields = new();

        private readonly List<CropProductionVariety> _cropProductionVarieties = new();

        public IReadOnlyCollection<CropProductionField> CropProductionFields => _cropProductionFields.AsReadOnly();

        public IReadOnlyCollection<CropProductionVariety> CropProductionVarieties => _cropProductionVarieties.AsReadOnly();

        private CropProduction()
        {
            // Required by EF
        }

        public CropProduction(string name, Guid cropId, Guid seasonId, decimal expectedYield, decimal? actualYield)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(cropId, nameof(cropId));
            Guard.AgainstNull(seasonId, nameof(seasonId));
            Guard.AgainstZero(expectedYield, nameof(expectedYield));

            Name = name;
            CropId = cropId;
            SeasonId = seasonId;
            ExpectedYield = expectedYield;
            ActualYield = actualYield;
        }

        public CropProduction(string name, Guid cropId, Guid seasonId, decimal expectedYield, decimal? actualYield, 
            List<CropProductionField> cropProductionFields, List<CropProductionVariety> cropProductionVarieties)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(cropId, nameof(cropId));
            Guard.AgainstNull(seasonId, nameof(seasonId));
            Guard.AgainstZero(expectedYield, nameof(expectedYield));
            Guard.AgainstNull(cropProductionFields, nameof(cropProductionFields));
            Guard.AgainstNull(cropProductionVarieties, nameof(cropProductionVarieties));

            Name = name;
            CropId = cropId;
            SeasonId = seasonId;
            ExpectedYield = expectedYield;
            ActualYield = actualYield;
            _cropProductionFields = cropProductionFields;
            _cropProductionVarieties = cropProductionVarieties;
        }

        public void AddCropProductionField(CropProductionField cropProductionField)
        {
            Guard.AgainstNull(cropProductionField, nameof(cropProductionField));

            _cropProductionFields.Add(cropProductionField);
        }

        public void AddCropProductionVariety(CropProductionVariety cropProductionVariety)
        {
            _cropProductionVarieties.Add(cropProductionVariety);
        }

        public void UpdateDetails(string name, decimal expectedYield, decimal? actualYield)
        {
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstZero(expectedYield, nameof(expectedYield));

            Name = name;
            ExpectedYield = expectedYield;
            ActualYield = actualYield;
        }
    }
}
