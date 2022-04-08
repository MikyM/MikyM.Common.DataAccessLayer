namespace MikyM.Common.DataAccessLayer.Repositories;

/// <summary>
/// Marker interface
/// </summary>
public interface IBaseRepository
{
    /// <summary>
    /// Entity type
    /// </summary>
    Type EntityType { get; }
}