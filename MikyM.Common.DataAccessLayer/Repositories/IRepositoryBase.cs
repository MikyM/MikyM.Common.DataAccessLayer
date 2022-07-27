namespace MikyM.Common.DataAccessLayer.Repositories;

/// <summary>
/// Defines a base repository.
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// The type of the entity held in this repository.
    /// </summary>
    Type EntityType { get; }
}
