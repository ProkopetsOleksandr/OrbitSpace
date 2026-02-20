namespace OrbitSpace.Application.Common.Models;

public abstract class EmailMessage
{
    public abstract string To { get; }
    public abstract string Subject { get; }
    public abstract string Body { get; }
}