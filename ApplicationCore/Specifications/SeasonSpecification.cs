using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Domain.Specifications
{
    public class SeasonSpecification : BaseSpecification<Season>
    {
        public SeasonSpecification(string searchQuery)
            : base(s => string.IsNullOrEmpty(searchQuery) || s.Name.Contains(searchQuery))
        {
        }

        public SeasonSpecification(int skip, int take, string searchQuery) 
            : base(s => string.IsNullOrEmpty(searchQuery) || s.Name.Contains(searchQuery))
        {
            AddInclude(s => s.SeasonStatus);
            ApplyOrderBy(s => s.Name);
            ApplyPaging(skip, take);
        }
    }
}
