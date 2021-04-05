using System;

namespace Edgias.Agrik.ApplicationCore.Entities
{
    public class CropProductionField : BaseEntity
    {
        public Guid FieldId { get; set; }

        public Field Field { get; set; }

        public Guid CropProductionId { get; set; }

        public CropProduction CropProduction { get; set; }
    }
}
