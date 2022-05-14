namespace MikyM.Common.DataAccessLayer.Repositories;

/// <summary>
/// Marker interface
/// </summary>
public interface IRepositoryBase
{
    /// <summary>
    /// Entity type
    /// </summary>
    Type EntityType { get; }
}