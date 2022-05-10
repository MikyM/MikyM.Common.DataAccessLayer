using System.Collections.Generic;
using Autofac;
using IdGen;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions;
using MikyM.Common.DataAccessLayer.Helpers;
using MikyM.Common.DataAccessLayer.Specifications.Validators;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// Configuration for Data Access Layer
/// </summary>
public class DataAccessConfiguration : IOptions<DataAccessConfiguration>
{
    /// <summary>
    /// Creates an instance of the configuration class
    /// </summary>
    /// <param name="builder"></param>
    public DataAccessConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }

    internal readonly ContainerBuilder Builder;
    private bool _disableOnBeforeSaveChanges = false;
    private Dictionary<string, Func<IUnitOfWork, Task>>? _onBeforeSaveChangesActions;

    /// <summary>
    /// Whether to cache include expressions (queries are evaluated faster).
    /// </summary>
    public bool EnableIncludeCache { get; set; } = false;

    /// <summary>
    /// Disables the insertion of audit log entries
    /// </summary>
    public bool DisableOnBeforeSaveChanges
    {
        get => _disableOnBeforeSaveChanges;
        set
        {
            _disableOnBeforeSaveChanges = value;
            SharedState.DisableOnBeforeSaveChanges = value;
        }
    }

    /// <summary>
    /// Actions to execute before each <see cref="IUnitOfWork.CommitAsync()"/>
    /// </summary>
    public Dictionary<string, Func<IUnitOfWork, Task>>? OnBeforeSaveChangesActions
         => _onBeforeSaveChangesActions;

    /// <summary>
    /// Adds an on before save changes action for a given context
    /// </summary>
    /// <param name="action">Action to perform</param>
    /// <typeparam name="TCotext">Type of the context for the action</typeparam>
    /// <exception cref="NotSupportedException">Throw when trying to register second action for same context type</exception>
    public void AddOnBeforeSaveChangesAction<TCotext>(Func<IUnitOfWork, Task> action)
        where TCotext : AuditableDbContext
    {
        _onBeforeSaveChangesActions ??= new Dictionary<string, Func<IUnitOfWork, Task>>();
        
        if (_onBeforeSaveChangesActions.TryGetValue(typeof(TCotext).Name, out _))
            throw new NotSupportedException("Multiple actions for same context aren't supported");
        
        _onBeforeSaveChangesActions.Add(typeof(TCotext).Name, action);
    }

    /// <summary>
    /// Instance of options
    /// </summary>
    public DataAccessConfiguration Value => this;
}