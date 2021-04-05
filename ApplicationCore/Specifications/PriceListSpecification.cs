using Edgias.Agrik.ApplicationCore.Entities;
using System;

namespace Edgias.Agrik.ApplicationCore.Specifications
{
    public class PriceListSpecification : BaseSpecification<PriceList>
    {
        public PriceListSpecification()
            : base(criteria: null)
        {
            AddInclude(pl => pl.Item);
            ApplyOrderBy(pl => pl.Item.Name);
        }

        public PriceListSpecification(Guid itemId)
            : base(pl => pl.ItemId == itemId)
        {
            ApplyOrderBy(pl => pl.Name);
        }

        public PriceListSpecification(string searchQuery)
            : base(pl => string.IsNullOrEmpty(searchQuery) || pl.Name.Contains(searchQuery) || pl.Item.Name.Contains(searchQuery))
        {
        }

        public PriceListSpecification(int skip, int take, string searchQuery) 
            : base(pl => string.IsNullOrEmpty(searchQuery) || pl.Name.Contains(searchQuery) || pl.Item.Name.Contains(searchQuery))
        {
            AddInclude(pl => pl.Item);
            ApplyOrderBy(pl => pl.Item.Name);
            ApplyPaging(skip, take);
        }
    }
}
