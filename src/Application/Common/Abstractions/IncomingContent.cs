namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public abstract class IncomingContent
{
    public abstract Stream Body { get; }
    public abstract string? ContentType { get; }
    public abstract long? ContentLength { get; }
    public abstract string? FirstHeader(string name);
    public abstract string? FirstQuery(string name);
}
