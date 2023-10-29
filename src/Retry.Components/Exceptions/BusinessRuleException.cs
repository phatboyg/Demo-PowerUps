using System.Runtime.Serialization;

namespace Retry.Components.Exceptions;

[Serializable]
public class BusinessRuleException :
    Exception
{
    public BusinessRuleException()
    {
    }

    protected BusinessRuleException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
    }

    public BusinessRuleException(string? message) 
        : base(message)
    {
    }

    public BusinessRuleException(string? message, Exception? innerException) 
        : base(message, innerException)
    {
    }
}