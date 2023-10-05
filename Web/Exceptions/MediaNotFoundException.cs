using System.Runtime.Serialization;

namespace Web.Exceptions;

public class MediaNotFoundException : Exception
{
    public MediaNotFoundException(string mediaKey)
    {
        MediaKey = mediaKey;
    }

    protected MediaNotFoundException(SerializationInfo info, StreamingContext context, string mediaKey) : base(info, context)
    {
        MediaKey = mediaKey;
    }

    public MediaNotFoundException(string? message, string mediaKey) : base(message)
    {
        MediaKey = mediaKey;
    }

    public MediaNotFoundException(string? message, Exception? innerException, string mediaKey) : base(message, innerException)
    {
        MediaKey = mediaKey;
    }

    public string MediaKey { get; set; }
}