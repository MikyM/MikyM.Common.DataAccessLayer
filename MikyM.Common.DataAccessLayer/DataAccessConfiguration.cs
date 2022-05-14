using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Options;

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
    internal List<IdGeneratorConfiguration>? IdGeneratorConfigurations;

    /// <summary>
    /// Instance of options
    /// </summary>
    public DataAccessConfiguration Value => this;
}