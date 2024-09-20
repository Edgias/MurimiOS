using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Domain.Specifications
{
    public class CropUnitSpecification : BaseSpecification<CropUnit>
    {
        public CropUnitSpecification(string searchQuery)
            : base(cu => string.IsNullOrEmpty(searchQuery) || cu.Name.Contains(searchQuery))
        {
        }

        public CropUnitSpecification(int skip, int take, string searchQuery) 
            : base(cu => string.IsNullOrEmpty(searchQuery) || cu.Name.Contains(searchQuery))
        {
            ApplyOrderBy(cu => cu.Name);
            ApplyPaging(skip, take);
        }
    }
}
