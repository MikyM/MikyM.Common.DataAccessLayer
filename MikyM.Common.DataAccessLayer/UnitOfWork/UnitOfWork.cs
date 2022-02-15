using Autofac;
using Autofac.Core;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;

namespace MikyM.Common.DataAccessLayer.UnitOfWork;

/// <summary>
/// Unit of work implementation
/// </summary>
/// <inheritdoc cref="IUnitOfWork{TContext}"/>
public sealed class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : AuditableDbContext
{
    /// <summary>
    /// Inner root <see cref="ILifetimeScope"/>
    /// </summary>
    private readonly ILifetimeScope _lifetimeScope;

    // To detect redundant calls
    private bool _disposed;
    // ReSharper disable once InconsistentNaming

    /// <summary>
    /// Repository cache
    /// </summary>
    private ConcurrentDictionary<string, IBaseRepository>? _repositories;

    /// <summary>
    /// Inner <see cref="IDbContextTransaction"/>
    /// </summary>
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Creates a new instance of <see cref="UnitOfWork{TContext}"/>
    /// </summary>
    /// <param name="context"><see cref="DbContext"/> to be used</param>
    /// <param name="lifetimeScope">Root <see cref="ILifetimeScope"/></param>
    public UnitOfWork(TContext context, ILifetimeScope lifetimeScope)
    {
        Context = context;
        _lifetimeScope = lifetimeScope;
    }

    /// <inheritdoc />
    public TContext Context { get; }

    /// <inheritdoc />
    public async Task UseTransaction()
    {
        _transaction ??= await Context.Database.BeginTransactionAsync();
    }

    /// <inheritdoc />
    public TRepository GetRepository<TRepository>() where TRepository : class, IBaseRepository
    {
        _repositories ??= new ConcurrentDictionary<string, IBaseRepository>();

        var type = typeof(TRepository);
        string name = type.FullName ?? throw new InvalidOperationException();

        if (_repositories.TryGetValue(name, out var repository)) return (TRepository) repository;

        if (_repositories.TryAdd(name,
                _lifetimeScope.Resolve<TRepository>(new ResolvedParameter(
                    (pi, _) => pi.ParameterType.IsAssignableTo(typeof(DbContext)), (_, _) => Context))))
            return (TRepository)_repositories[name];

        if (_repositories.TryGetValue(name, out repository)) return (TRepository) repository;

        throw new InvalidOperationException(
            $"Repository of type {name} couldn't be added to and/or retrieved.");
    }

    /// <inheritdoc />
    public async Task RollbackAsync()
    {
        if (_transaction is not null) await _transaction.RollbackAsync();
    }

    /// <inheritdoc />
    public async Task<int> CommitAsync()
    {
        int result = await Context.SaveChangesAsync();
        if (_transaction is not null) await _transaction.CommitAsync();
        return result;
    }

    /// <inheritdoc />
    public async Task<int> CommitAsync(string? userId)
    {
        int result = await Context.SaveChangesAsync(userId);
        if (_transaction is not null) await _transaction.CommitAsync();
        return result;
    }

    // Public implementation of Dispose pattern callable by consumers.
    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            Context.Dispose();
            _transaction?.Dispose();
        }

        _repositories = null;

        _disposed = true;
    }
}