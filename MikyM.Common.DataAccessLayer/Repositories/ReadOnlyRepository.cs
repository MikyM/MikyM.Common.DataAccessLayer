using System;
using Microsoft.EntityFrameworkCore;
using MikyM.Common.DataAccessLayer.Filters;
using MikyM.Common.DataAccessLayer.Specifications;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MikyM.Common.DataAccessLayer.Repositories
{
    public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        protected DbContext _context;
        protected readonly DbSet<TEntity> _set;

        protected ReadOnlyRepository(DbContext context)
        {
            _context = _context ?? throw new ArgumentNullException(nameof(_context));
            _set = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetAsync(params object[] keyValues)
        {
            return await _set.FindAsync(keyValues);
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(
            ISpecifications<TEntity> baseSpecifications = null)
        {
            return await SpecificationEvaluator<TEntity>
                .GetQuery(_context.Set<TEntity>().AsQueryable(), baseSpecifications)
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(PaginationFilter filter,
            ISpecifications<TEntity> baseSpecifications = null)
        {
            return await SpecificationEvaluator<TEntity>
                .GetQuery(_context.Set<TEntity>().AsQueryable(), baseSpecifications)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public virtual async Task<long> CountAsync()
        {
            return await _context.Set<TEntity>().AsNoTracking().LongCountAsync();
        }

        public virtual async Task<long> CountWhereAsync(ISpecifications<TEntity> specifications = null)
        {
            return await SpecificationEvaluator<TEntity>.GetQuery(_context.Set<TEntity>().AsQueryable(), specifications)
                .AsNoTracking()
                .LongCountAsync();
        }
    }
}
