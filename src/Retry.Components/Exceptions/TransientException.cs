using System.Runtime.Serialization;

namespace Retry.Components.Exceptions;

[Serializable]
public class TransientException :
    Exception
{
    public TransientException()
    {
    }

    protected TransientException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }

    public TransientException(string? message) 
        : base(message)
    {
    }

    public TransientException(string? message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}