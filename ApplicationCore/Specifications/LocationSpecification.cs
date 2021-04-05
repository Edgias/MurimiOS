using Edgias.Agrik.ApplicationCore.Entities;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class LocationSpecification : BaseSpecification<Location>
    {
        public LocationSpecification(string searchQuery)
            : base(l => string.IsNullOrEmpty(searchQuery) ||
            l.Name.Contains(searchQuery) ||
            l.Street1.Contains(searchQuery) ||
            l.Street2.Contains(searchQuery) ||
            l.State.Contains(searchQuery) ||
            l.Country.Contains(searchQuery))
        {
        }

        public LocationSpecification(int skip, int take, string searchQuery) 
            : base(l => string.IsNullOrEmpty(searchQuery) || 
            l.Name.Contains(searchQuery) || 
            l.Street1.Contains(searchQuery) || 
            l.Street2.Contains(searchQuery) || 
            l.State.Contains(searchQuery) || 
            l.Country.Contains(searchQuery))
        {
            ApplyOrderBy(l => l.Name);
            ApplyPaging(skip, take);
        }
    }
}
