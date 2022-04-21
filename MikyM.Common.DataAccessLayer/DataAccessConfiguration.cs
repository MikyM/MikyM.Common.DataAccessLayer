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
        this._builder = builder;
    }

    private readonly ContainerBuilder _builder;
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
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <typeparam name="TEvaluator">Type to register</typeparam>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public DataAccessConfiguration AddEvaluator<TEvaluator>() where TEvaluator : class, IEvaluator
    {
        _builder.RegisterType(typeof(TEvaluator))
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return this;
    }

    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <param name="evaluator">Type of the custom evaluator</param>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public DataAccessConfiguration AddEvaluator(Type evaluator)
    {
        if (evaluator.GetInterface(nameof(IEvaluator)) is null)
            throw new NotSupportedException("Registered evaluator did not implement IEvaluator interface");

        _builder.RegisterType(evaluator)
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return this;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IInMemoryEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public DataAccessConfiguration AddInMemoryEvaluators()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            _builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null)
                .As<IInMemoryEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return this;
    }

    /// <summary>
    /// Adds all validators that implement <see cref="IValidator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public DataAccessConfiguration AddValidators()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            _builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IValidator)) is not null)
                .As<IValidator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return this;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public DataAccessConfiguration AddEvaluators()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == typeof(IncludeEvaluator).Assembly)
                continue;

            _builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null)
                .As<IEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return this;
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
    /// Gets or sets settings for <see cref="IdGenerator"/>
    /// </summary>
    public IdGeneratorOptions? IdGeneratorOptions { get; set; }
    /// <summary>
    /// Gets or sets the Id for <see cref="IdGenerator"/>
    /// </summary>
    public int IdGeneratorId { get; set; }

    /// <summary>
    /// Instance of options
    /// </summary>
    public DataAccessConfiguration Value => this;
}