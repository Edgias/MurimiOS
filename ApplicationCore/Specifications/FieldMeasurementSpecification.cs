using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Domain.Specifications
{
    public class FieldMeasurementSpecification : BaseSpecification<FieldMeasurement>
    {
        public FieldMeasurementSpecification(string searchQuery)
            : base(fm => string.IsNullOrEmpty(searchQuery) || fm.Name.Contains(searchQuery))
        {
        }

        public FieldMeasurementSpecification(int skip, int take, string searchQuery) 
            : base(fm => string.IsNullOrEmpty(searchQuery) || fm.Name.Contains(searchQuery))
        {
            ApplyOrderBy(fm => fm.Name);
            ApplyPaging(skip, take);
        }
    }
}
