using System.Threading.Tasks;
using MikyM.Common.DataAccessLayer.Repositories;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// Base definition of a unit of work
/// </summary>
public interface IUnitOfWorkBase : IDisposable
{
    /// <summary>
    /// Gets a repository of a given type
    /// </summary>
    /// <typeparam name="TRepository">Type of the repository to get</typeparam>
    /// <returns>Wanted repository</returns>
    TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;
    /// <summary>
    /// Commits changes
    /// </summary>
    /// <returns>Number of affected rows</returns>
    Task CommitAsync();
    /// <summary>
    /// Commits changes
    /// </summary>
    /// <param name="userId">Id of the user that is responsible for doing changes</param>
    /// <returns>Number of affected rows</returns>
    Task CommitAsync(string userId);
    /// <summary>
    /// Rolls the transaction back
    /// </summary>
    /// <returns>Task representing the asynchronous operation</returns>
    Task RollbackAsync();
}