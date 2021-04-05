using Murimi.ApplicationCore.Entities;
using System;

namespace Murimi.ApplicationCore.Specifications
{
    public class CropVarietySpecification : BaseSpecification<CropVariety>
    {
        public CropVarietySpecification(string searchQuery)
            : base(cv => string.IsNullOrEmpty(searchQuery))
        {
        }

        public CropVarietySpecification(Guid cropId)
            : base(cv => cv.CropId == cropId)
        {
        }

        public CropVarietySpecification(Guid cropId, string searchQuery)
            : base(cv => cv.CropId == cropId && string.IsNullOrEmpty(searchQuery))
        {
        }

        public CropVarietySpecification(Guid cropId, int skip, int take, string searchQuery)
            : base(cv => cv.CropId == cropId && string.IsNullOrEmpty(searchQuery))
        {
            ApplyOrderBy(cv => cv.Name);
            ApplyPaging(skip, take);
        }

        public CropVarietySpecification(int skip, int take, string searchQuery) 
            : base(cv => string.IsNullOrEmpty(searchQuery))
        {
            AddInclude(cv => cv.Crop);
            ApplyOrderBy(cv => cv.Name);
            ApplyPaging(skip, take);
        }
    }
}
