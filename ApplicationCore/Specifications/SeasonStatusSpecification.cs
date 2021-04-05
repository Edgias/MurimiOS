using Murimi.ApplicationCore.Entities;

namespace Murimi.ApplicationCore.Specifications
{
    public class SeasonStatusSpecification : BaseSpecification<SeasonStatus>
    {
        public SeasonStatusSpecification(bool isDefault = true)
            : base(ss => ss.IsDefault == isDefault)
        {
        }

        public SeasonStatusSpecification(string searchQuery)
            : base(ss => string.IsNullOrEmpty(searchQuery) || ss.Name.Contains(searchQuery))
        {
        }

        public SeasonStatusSpecification(int skip, int take, string searchQuery) 
            : base(ss => string.IsNullOrEmpty(searchQuery) || ss.Name.Contains(searchQuery))
        {
            ApplyOrderBy(ss => ss.Name);
            ApplyPaging(skip, take);
        }
    }
}
