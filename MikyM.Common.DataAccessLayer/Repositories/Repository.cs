﻿using System.Collections.Generic;
using MikyM.Common.DataAccessLayer.Exceptions;
using MikyM.Common.DataAccessLayer.Helpers;
using MikyM.Common.Domain.Entities;

namespace MikyM.Common.DataAccessLayer.Repositories;

/// <inheritdoc cref="IRepository{TEntity}"/>
public class Repository<TEntity> : ReadOnlyRepository<TEntity>, IRepository<TEntity>
    where TEntity : AggregateRootEntity
{
    internal Repository(DbContext context, ISpecificationEvaluator specificationEvaluator) : base(context,
        specificationEvaluator)
    {
    }

    public virtual void Add(TEntity entity)
    {
        Context.Set<TEntity>().Add(entity);
    }

    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().AddRange(entities);
    }

    public virtual void BeginUpdate(TEntity entity, bool shouldSwapAttached = false)
    {
        var local = Context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

        if (local is not null && shouldSwapAttached)
        {
            Context.Entry(local).State = EntityState.Detached;
        }
        else if (local is not null && !shouldSwapAttached)
        {
            return;
        }

        Context.Attach(entity);
    }

    public virtual void BeginUpdateRange(IEnumerable<TEntity> entities, bool shouldSwapAttached = false)
    {
        foreach (var entity in entities)
        {
            var local = Context.Set<TEntity>().Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            if (local is not null && shouldSwapAttached)
            {
                Context.Entry(local).State = EntityState.Detached;
            }
            else if (local is not null && !shouldSwapAttached)
            {
                return;
            }

            Context.Attach(entity);
        }
    }

    public virtual void Delete(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public virtual void Delete(long id)
    {
        var entity = Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!;
        Context.Set<TEntity>().Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public virtual void DeleteRange(IEnumerable<long> ids)
    {
        var entities = ids.Select(id =>
                Context.FindTracked<TEntity>(id) ?? (TEntity) Activator.CreateInstance(typeof(TEntity), id)!)
            .ToList();
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public virtual void Disable(TEntity entity)
    {
        BeginUpdate(entity);
        entity.IsDisabled = true;
    }

    public virtual async Task DisableAsync(long id)
    {
        var entity = await GetAsync(id);
        BeginUpdate(entity ?? throw new NotFoundException());
        entity.IsDisabled = true;
    }

    public virtual void DisableRange(IEnumerable<TEntity> entities)
    {
        var aggregateRootEntities = entities.ToList();
        BeginUpdateRange(aggregateRootEntities);
        foreach (var entity in aggregateRootEntities) entity.IsDisabled = true;
    }

    public virtual async Task DisableRangeAsync(IEnumerable<long> ids)
    {
        var entities = await Context.Set<TEntity>()
            .Join(ids, ent => ent.Id, id => id, (ent, id) => ent)
            .ToListAsync();
        BeginUpdateRange(entities);
        entities.ForEach(ent => ent.IsDisabled = true);
    }
}