using System.Runtime.Serialization;

namespace Web.Exceptions;

public class ServerUnreachableException : Exception
{
    public ServerUnreachableException(string serverName)
    {
        ServerName = serverName;
    }

    protected ServerUnreachableException(SerializationInfo info, StreamingContext context, string serverName) : base(info, context)
    {
        ServerName = serverName;
    }

    public ServerUnreachableException(string? message, string serverName) : base(message)
    {
        ServerName = serverName;
    }

    public ServerUnreachableException(string? message, Exception? innerException, string serverName) : base(message, innerException)
    {
        ServerName = serverName;
    }

    public string ServerName { get; set; }
}