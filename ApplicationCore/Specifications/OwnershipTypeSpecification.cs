using Murimi.ApplicationCore.Entities;

namespace Murimi.ApplicationCore.Specifications
{
    public class OwnershipTypeSpecification : BaseSpecification<OwnershipType>
    {
        public OwnershipTypeSpecification(string searchQuery)
            : base(ot => string.IsNullOrEmpty(searchQuery) || ot.Name.Contains(searchQuery))
        {
        }

        public OwnershipTypeSpecification(int skip, int take, string searchQuery) 
            : base(ot => string.IsNullOrEmpty(searchQuery) || ot.Name.Contains(searchQuery))
        {
            ApplyOrderBy(ot => ot.Name);
            ApplyPaging(skip, take);
        }
    }
}
