
namespace MikyM.Common.DataAccessLayer.Exceptions;

public class IdGeneratorNameNotUniqueException : Exception
{
    public IdGeneratorNameNotUniqueException()
    {
    }

    public IdGeneratorNameNotUniqueException(string message)
        : base(message)
    {
    }

    public IdGeneratorNameNotUniqueException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
