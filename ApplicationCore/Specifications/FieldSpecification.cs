using Edgias.Agrik.ApplicationCore.Entities;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class FieldSpecification : BaseSpecification<Field>
    {
        public FieldSpecification(string searchQuery)
            : base(f => string.IsNullOrEmpty(searchQuery) || f.Name.Contains(searchQuery))
        {
        }

        public FieldSpecification(int skip, int take, string searchQuery) 
            : base(f => string.IsNullOrEmpty(searchQuery) || f.Name.Contains(searchQuery))
        {
            AddInclude(f => f.Location);
            AddInclude(f => f.FieldMeasurement);
            AddInclude(f => f.SoilType);
            AddInclude(f => f.OwnershipType);
            ApplyOrderBy(f => f.Name);
            ApplyPaging(skip, take);
        }
    }
}
