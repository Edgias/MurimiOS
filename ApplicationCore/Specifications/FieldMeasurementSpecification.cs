using Edgias.Agrik.ApplicationCore.Entities;

namespace Edgias.Agrik.ApplicationCore.Specifications
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
