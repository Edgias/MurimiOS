using Murimi.ApplicationCore.Entities;

namespace Murimi.ApplicationCore.Specifications
{
    public class ItemCategorySpecification : BaseSpecification<ItemCategory>
    {
        public ItemCategorySpecification()
            : base(criteria: null)
        {
            ApplyOrderBy(p => p.Name);
        }

        public ItemCategorySpecification(string searchQuery)
            : base(ic => string.IsNullOrEmpty(searchQuery) || ic.Name.Contains(searchQuery))
        {
        }

        public ItemCategorySpecification(int skip, int take, string searchQuery) 
            : base(ic => string.IsNullOrEmpty(searchQuery) || ic.Name.Contains(searchQuery))
        {
            ApplyOrderBy(pc => pc.Name);
            ApplyPaging(skip, take);
        }
    }
}
