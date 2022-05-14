using System.IO;
using System.Threading;
using IdGen;

namespace MikyM.Common.DataAccessLayer;

/// <summary>
/// <see cref="IdGenerator"/> configuration
/// </summary>
public class IdGeneratorConfiguration
{
    /// <summary>
    /// Name for the generator if registering more than one, must be unique, defaults to "SnowflakeIdGenerator"
    /// </summary>
    public string Name { get; set; } = "SnowflakeIdGenerator";
    
    /// <summary>
    /// Gets or sets the sequence overflow strategy, defaults to <see cref="SpinWait"/>
    /// </summary>
    public SequenceOverflowStrategy SequenceOverflowStrategy { get; set; } = SequenceOverflowStrategy.SpinWait;

    /// <summary>
    /// Gets or sets the generator Id, defaults to 1
    /// </summary>
    public int GeneratorId { get; set; } = 1;
    
    /// <summary>
    /// Gets or sets the <see cref="IdStructure"/> for the generator
    /// </summary>
    public IdStructure? IdStructure { get; set; }
    
    /// <summary>
    /// Gets or sets the <see cref="DefaultTimeSource"/> for the generator
    /// </summary>
    public DefaultTimeSource? DefaultTimeSource { get; set; }

    internal void Validate()
    {
        if (GeneratorId <= 0)
            throw new InvalidOperationException("Generator Id must be equal or bigger than 1");
        if (IdStructure is null)
            throw new InvalidOperationException("Generator's Id structure must be set (not null)");
        if (DefaultTimeSource is null)
            throw new InvalidOperationException("Generator's default time source must be set (not null)");
    }
}