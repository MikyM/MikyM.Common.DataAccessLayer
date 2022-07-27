using IdGen;

namespace MikyM.Common.DataAccessLayer.Exceptions;

/// <summary>
/// Exception thrown when the name of an <see cref="IdGenerator"/> is not unique.
/// </summary>]
internal class IdGeneratorNameNotUniqueException : Exception
{
    /// <summary>
    /// Base constructor.
    /// </summary>
    /// <param name="message">Message associated with the exception.</param>
    public IdGeneratorNameNotUniqueException(string message)
        : base(message)
    {
    }
}
