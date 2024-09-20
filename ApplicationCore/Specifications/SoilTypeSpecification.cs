﻿using Edgias.MurimiOS.Domain.Entities;

namespace Edgias.MurimiOS.Domain.Specifications
{
    public class SoilTypeSpecification : BaseSpecification<SoilType>
    {
        public SoilTypeSpecification(string searchQuery)
            : base(st => string.IsNullOrEmpty(searchQuery) || st.Name.Contains(searchQuery))
        {
        }

        public SoilTypeSpecification(int skip, int take, string searchQuery) 
            : base(st => string.IsNullOrEmpty(searchQuery) || st.Name.Contains(searchQuery))
        {
            ApplyOrderBy(st => st.Name);
            ApplyPaging(skip, take);
        }
    }
}
