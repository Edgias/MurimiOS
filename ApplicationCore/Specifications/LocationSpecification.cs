using Murimi.ApplicationCore.Entities;

namespace Murimi.ApplicationCore.Specifications
{
    public class LocationSpecification : BaseSpecification<Location>
    {
        public LocationSpecification(string searchQuery)
            : base(l => string.IsNullOrEmpty(searchQuery) ||
            l.Name.Contains(searchQuery) ||
            l.LocationAddress.Street1.Contains(searchQuery) ||
            l.LocationAddress.Street2.Contains(searchQuery) ||
            l.LocationAddress.State.Contains(searchQuery) ||
            l.LocationAddress.Country.Contains(searchQuery))
        {
        }

        public LocationSpecification(int skip, int take, string searchQuery) 
            : base(l => string.IsNullOrEmpty(searchQuery) || 
            l.Name.Contains(searchQuery) || 
            l.LocationAddress.Street1.Contains(searchQuery) || 
            l.LocationAddress.Street2.Contains(searchQuery) || 
            l.LocationAddress.State.Contains(searchQuery) || 
            l.LocationAddress.Country.Contains(searchQuery))
        {
            ApplyOrderBy(l => l.Name);
            ApplyPaging(skip, take);
        }
    }
}
