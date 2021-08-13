using System.Collections.Generic;
using System.Threading.Tasks;
using MikyM.Common.DataAccessLayer.Filters;
using MikyM.Common.DataAccessLayer.Specifications;

namespace MikyM.Common.DataAccessLayer.Repositories
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> GetAsync(params object[] keyValues);
        Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(PaginationFilter filter,
            ISpecifications<TEntity> specifications = null);

        Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(ISpecifications<TEntity> specifications = null);
        Task<long> CountAsync();
        Task<long> CountWhereAsync(ISpecifications<TEntity> specifications = null);
    }
}
