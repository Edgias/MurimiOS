using Edgias.MurimiOS.Domain.Entities;
using Edgias.MurimiOS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edgias.MurimiOS.Domain.SharedKernel;
using System.Threading;

namespace Edgias.MurimiOS.Infrastructure.Data.Repositories
{
    public class EfRepository<T> : IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
    {
        protected readonly MurimiDbContext _dbContext;

        public EfRepository(MurimiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            object[] keyValues = new object[] { id };

            return await _dbContext.Set<T>().FindAsync(keyValues, cancellationToken);
        }

        public virtual Task<T> GetSingleBySpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            return ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).ToListAsync(cancellationToken);
        }

        public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(spec).CountAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Add(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual async Task AddAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().AddRange(entities);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Remove(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
        {
            var list = await GetAsync(specification, cancellationToken);

            return list.Any();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
        }

    }
}

