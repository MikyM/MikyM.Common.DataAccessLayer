using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IdGen;
using MikyM.Common.DataAccessLayer.Exceptions;
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
    /// <param name="builder">Current instance of <see cref="ContainerBuilder"/></param>
    /// <param name="options"><see cref="Action"/> that configures DAL.</param>
    public static ContainerBuilder AddDataAccessLayer(this ContainerBuilder builder, Action<DataAccessConfiguration> options)
    {
        var config = new DataAccessConfiguration(builder);
        options.Invoke(config);
        
        return builder;
    }

    /// <summary>
    /// Adds default factory method for <see cref="IdGeneratorFactory"/>
    /// </summary>
    /// <param name="provider">Current instance of <see cref="IServiceProvider"/></param>
    /// <returns>Current instance of <see cref="IServiceProvider"/></returns>
    public static IServiceProvider ConfigureIdGeneratorFactory(this IServiceProvider provider)
    {
        IdGeneratorFactory.AddFactoryMethod(() => provider.GetAutofacRoot().Resolve<IdGenerator>());
        return provider;
    }

    /// <summary>
    /// Adds default factory method for <see cref="IdGeneratorFactory"/>
    /// </summary>
    /// <param name="provider">Current instance of <see cref="IServiceProvider"/></param>
    /// <param name="generatorName">Generator name</param>
    /// <returns>Current instance of <see cref="IServiceProvider"/></returns>
    public static IServiceProvider ConfigureIdGeneratorFactory(this IServiceProvider provider, string generatorName)
    {
        IdGeneratorFactory.AddFactoryMethod(() => provider.GetAutofacRoot().Resolve<IdGenerator>(), generatorName);
        return provider;
    }
    
    /// <summary>
    /// Registers <see cref="IdGenerator"/> with the container, if it's the first generator being added it will be also registered AsSelf(), aside from AsName(options.Name)
    /// </summary>
    /// <param name="dataAccessOptions"></param>
    /// <param name="options">Id generator configuration</param>
    /// <returns>Current <see cref="DataAccessConfiguration"/> instance</returns>
    public static DataAccessConfiguration AddSnowflakeIdGenerator(this DataAccessConfiguration dataAccessOptions, Action<IdGeneratorConfiguration> options)
    {
        var opt = new IdGeneratorConfiguration();
        options(opt);
        opt.Validate();

        dataAccessOptions.IdGeneratorConfigurations ??= new List<IdGeneratorConfiguration>();
        if (dataAccessOptions.IdGeneratorConfigurations.Any(x => x.Name == opt.Name))
            throw new IdGeneratorNameNotUniqueException("Generator's name must be unique");
        
        dataAccessOptions.IdGeneratorConfigurations.Add(opt);

        if (dataAccessOptions.IdGeneratorConfigurations.Count == 1)
            dataAccessOptions.Builder.Register(_ => new IdGenerator(opt.GeneratorId,
                    new IdGeneratorOptions(opt.IdStructure, opt.DefaultTimeSource, opt.SequenceOverflowStrategy)))
                .AsSelf()
                .Named<IdGenerator>(opt.Name)
                .SingleInstance();
        else
            dataAccessOptions.Builder.Register(_ => new IdGenerator(opt.GeneratorId,
                    new IdGeneratorOptions(opt.IdStructure, opt.DefaultTimeSource, opt.SequenceOverflowStrategy)))
                .Named<IdGenerator>(opt.Name)
                .SingleInstance();

        return dataAccessOptions;
    }
}