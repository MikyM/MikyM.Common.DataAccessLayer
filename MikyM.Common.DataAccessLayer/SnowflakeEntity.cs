using MikyM.Common.Domain;
using MikyM.Common.Domain.Entities;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// A snowflake entity.
/// </summary>
[PublicAPI]
public class SnowflakeEntity : Entity, ISnowflakeEntity
{
    /// <inheritdoc/>
    public override long Id { get; protected set; } = IdGeneratorFactory.Build().CreateId();
}
