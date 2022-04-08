using MikyM.Common.Domain.Entities;

namespace MikyM.Common.DataAccessLayer.Repositories;

/// <summary>
/// Marker interface
/// </summary>
public interface IBaseRepository
{
}

/// <summary>
/// Marker interface
/// </summary>
public interface IBaseRepository<TEntity> where TEntity : AggregateRootEntity
{
    /// <summary>
    /// Entity type
    /// </summary>
    Type EntityType { get; }
}