using System.Net;

namespace Shared.Exceptions;

public class InternalServerException : Exception
{
    public InternalServerException(string message) : base(message)
    {

    }
    public InternalServerException(): base()
    {
        
    }
}