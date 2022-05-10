using Autofac;
using MikyM.Autofac.Extensions;
using MikyM.Common.DataAccessLayer.Specifications;
using MikyM.Common.DataAccessLayer.Specifications.Validators;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using IdGen;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MikyM.Common.DataAccessLayer.Pagination;
using MikyM.Common.Domain;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// DI extensions for <see cref="ContainerBuilder"/>
/// </summary>
public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers <see cref="IRepository{TEntity}"/>, <see cref="IReadOnlyRepository{TEntity}"/>, all base <see cref="IEvaluator"/> types, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork{TContext}"/> with <see cref="ContainerBuilder"/>.
    /// </remarks>
    /// <param name="builder">Current instance of <see cref="ContainerBuilder"/></param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static ContainerBuilder AddDataAccessLayer(this ContainerBuilder builder, Action<DataAccessConfiguration>? options = null)
    {
        var config = new DataAccessConfiguration(builder);
        options?.Invoke(config);

        var ctorFinder = new AllConstructorsFinder();

        builder.Register(x => config).As<IOptions<DataAccessConfiguration>>().SingleInstance();

        builder.RegisterGeneric(typeof(ReadOnlyRepository<>))
            .As(typeof(IReadOnlyRepository<>))
            .FindConstructorsWith(ctorFinder)
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(Repository<>))
            .As(typeof(IRepository<>))
            .FindConstructorsWith(ctorFinder)
            .InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(UnitOfWork<>)).As(typeof(IUnitOfWork<>)).InstancePerLifetimeScope();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null && x != typeof(IncludeEvaluator))
                .As<IEvaluator>()
                .FindConstructorsWith(ctorFinder)
                .SingleInstance();
        }

        builder.RegisterType<IncludeEvaluator>()
            .As<IEvaluator>()
            .UsingConstructor(typeof(bool))
            .FindConstructorsWith(ctorFinder)
            .WithParameter(new TypedParameter(typeof(bool), config.EnableIncludeCache))
            .SingleInstance();

        builder.RegisterType<ProjectionEvaluator>()
            .As<IProjectionEvaluator>()
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<SpecificationEvaluator>()
            .As<ISpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IEvaluator>), typeof(IProjectionEvaluator))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<SpecificationValidator>()
            .As<ISpecificationValidator>()
            .UsingConstructor(typeof(IEnumerable<IValidator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        builder.RegisterType<InMemorySpecificationEvaluator>()
            .As<IInMemorySpecificationEvaluator>()
            .UsingConstructor(typeof(IEnumerable<IInMemoryEvaluator>))
            .FindConstructorsWith(ctorFinder)
            .SingleInstance();

        return builder;
    }

    /// <summary>
    /// Adds Data Access Layer to the application.
    /// </summary>
    /// <remarks>
    /// Automatically registers <see cref="IRepository{TEntity}"/>, <see cref="IReadOnlyRepository{TEntity}"/>, all base <see cref="IEvaluator"/> types, <see cref="ISpecificationValidator"/>, <see cref="IInMemorySpecificationEvaluator"/>, <see cref="ISpecificationEvaluator"/>, <see cref="IUnitOfWork{TContext}"/> with <see cref="ContainerBuilder"/>.
    /// </remarks>
    /// <param name="provider">Current instance of <see cref="IServiceProvider"/></param>
    public static IServiceProvider ConfigureIdGeneratorFactory(this IServiceProvider provider)
    {
        IdGeneratorFactory.SetFactory(() => provider.GetAutofacRoot().Resolve<IdGenerator>());

        return provider;
    }
    
    /// <summary>
    /// Registers services required for pagination
    /// </summary>
    /// <param name="options">Data access configuration</param>
    /// <returns>Current instance of the <see cref="DataAccessConfiguration"/></returns>
    public static DataAccessConfiguration AddPaginationServices(this DataAccessConfiguration options)
    {
        options.Builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        options.Builder.Register(x =>
            {
                var accessor = x.Resolve<IHttpContextAccessor>();
                var request = accessor.HttpContext?.Request;
                var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
                return new UriService(uri);
            })
            .As<IUriService>()
            .SingleInstance();
        
        return options;
    }
    
    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <typeparam name="TEvaluator">Type to register</typeparam>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddEvaluator<TEvaluator>(this DataAccessConfiguration dataAccessOptions) where TEvaluator : class, IEvaluator
    {
        dataAccessOptions.Builder.RegisterType(typeof(TEvaluator))
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return dataAccessOptions;
    }

    /// <summary>
    /// Adds a given custom evaluator that implements <see cref="IEvaluator"/> interface.
    /// </summary>
    /// <param name="dataAccessOptions"></param>
    /// <param name="evaluator">Type of the custom evaluator</param>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddEvaluator(this DataAccessConfiguration dataAccessOptions, Type evaluator)
    {
        if (evaluator.GetInterface(nameof(IEvaluator)) is null)
            throw new NotSupportedException("Registered evaluator did not implement IEvaluator interface");

        dataAccessOptions.Builder.RegisterType(evaluator)
            .As<IEvaluator>()
            .FindConstructorsWith(new AllConstructorsFinder())
            .SingleInstance();

        return dataAccessOptions;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IInMemoryEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddInMemoryEvaluators(this DataAccessConfiguration dataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            dataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IInMemoryEvaluator)) is not null)
                .As<IInMemoryEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return dataAccessOptions;
    }

    /// <summary>
    /// Adds all validators that implement <see cref="IValidator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddValidators(this DataAccessConfiguration dataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            dataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IValidator)) is not null)
                .As<IValidator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return dataAccessOptions;
    }

    /// <summary>
    /// Adds all evaluators that implement <see cref="IEvaluator"/> from all assemblies.
    /// </summary>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddEvaluators(this DataAccessConfiguration dataAccessOptions)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly == typeof(IncludeEvaluator).Assembly)
                continue;

            dataAccessOptions.Builder.RegisterAssemblyTypes(assembly)
                .Where(x => x.GetInterface(nameof(IEvaluator)) is not null)
                .As<IEvaluator>()
                .FindConstructorsWith(new AllConstructorsFinder())
                .SingleInstance();
        }

        return dataAccessOptions;
    }

    /// <summary>
    /// Registers <see cref="IdGenerator"/> with the container
    /// </summary>
    /// <param name="dataAccessOptions"></param>
    /// <param name="options">Id generator configuration</param>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddSnowflakeIdGenerator(this DataAccessConfiguration dataAccessOptions, Action<IdGeneratorConfiguration> options)
    {
        var opt = new IdGeneratorConfiguration();
        options(opt);
        opt.Validate();

        dataAccessOptions.Builder.Register(_ => new IdGenerator(opt.GeneratorId,
                new IdGeneratorOptions(opt.IdStructure, opt.DefaultTimeSource, opt.SequenceOverflowStrategy)))
            .AsSelf()
            .SingleInstance();

        return dataAccessOptions;
    }
}