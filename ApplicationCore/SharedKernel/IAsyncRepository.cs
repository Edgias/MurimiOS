using Edgias.MurimiOS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Edgias.MurimiOS.Domain.SharedKernel
{
    public interface IAsyncRepository<T> where T: BaseEntity, IAggregateRoot
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        Task<int> CountAllAsync(CancellationToken cancellationToken = default);

        Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        Task<T> GetSingleBySpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    }
}
