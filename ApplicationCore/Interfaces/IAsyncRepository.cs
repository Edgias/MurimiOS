using Murimi.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Murimi.ApplicationCore.Interfaces
{
    public interface IAsyncRepository<T> where T: BaseEntity 
    {
        Task<T> AddAsync(T entity);

        Task AddAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task<bool> ExistsAsync(ISpecification<T> specification);

        Task<int> CountAllAsync();

        Task<int> CountAsync(ISpecification<T> specification);

        Task<T> GetSingleBySpecificationAsync(ISpecification<T> specification);

        Task<T> GetByIdAsync(Guid id);

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification);
    }
}
