using Autofac;
using MikyM.Autofac.Extensions;
using MikyM.Common.DataAccessLayer.Helpers;
using MikyM.Common.DataAccessLayer.Specifications.Validators;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// Options for Data Access Layer.
/// </summary>
public class DataAccessOptions
{
    public DataAccessOptions(ContainerBuilder builder)
    {
        this._builder = builder;
    }

    private readonly ContainerBuilder _builder;
    private bool _disableOnBeforeSaveChanges = false;

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
    /// <returns>Current <see cref="DataAccessOptions"/> instance</returns>
    public DataAccessOptions AddEvaluator<TEvaluator>() where TEvaluator : class, IEvaluator
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
    /// <returns>Current <see cref="DataAccessOptions"/> instance</returns>
    public DataAccessOptions AddEvaluator(Type evaluator)
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
    /// <returns>Current <see cref="DataAccessOptions"/> instance</returns>
    public DataAccessOptions AddInMemoryEvaluators()
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
    /// <returns>Current <see cref="DataAccessOptions"/> instance</returns>
    public DataAccessOptions AddValidators()
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
    /// <returns>Current <see cref="DataAccessOptions"/> instance</returns>
    public DataAccessOptions AddEvaluators()
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
}