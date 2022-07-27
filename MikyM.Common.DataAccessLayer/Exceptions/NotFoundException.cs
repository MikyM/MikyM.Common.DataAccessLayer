
namespace MikyM.Common.DataAccessLayer.Exceptions;

/// <summary>
/// Exception thrown when an entity couldn't be found.
/// </summary>
[PublicAPI]
public class NotFoundException : Exception
{
    /// <summary>
    /// Base constructor.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="message">Message associated with the exception.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="message">Message associated with the exception.</param>
    /// <param name="inner">Inner exception.</param>
    public NotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
