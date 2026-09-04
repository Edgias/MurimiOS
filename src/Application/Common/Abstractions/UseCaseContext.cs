namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class UseCaseContext(ICurrentUser user, IResponseMetadata response)
{
    public ICurrentUser User { get; } = user;
    public IResponseMetadata Response { get; } = response;
    public IDictionary<object, object?> State { get; } = new Dictionary<object, object?>();
}
