﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace MikyM.Common.DataAccessLayer.Repositories
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void AddOrUpdate(TEntity entity);
        void AddOrUpdateRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Delete(TEntity entity);
        void Delete(long id);
        void DeleteRange(IEnumerable<TEntity> entities);
        void DeleteRange(IEnumerable<long> ids);
    }
}
