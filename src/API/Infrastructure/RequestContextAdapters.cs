using System.Net;
using System.Security.Claims;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.API.Infrastructure;
using ICurrentUser = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ICurrentUser;
using IncomingContent = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IncomingContent;
using IResponseMetadata = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IResponseMetadata;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private HttpContext Context => accessor.HttpContext ?? throw new InvalidOperationException("No active request.");
    public ClaimsPrincipal Principal => Context.User;
    public string CorrelationId => Context.TraceIdentifier;
    public IPAddress? RemoteAddress => Context.Connection.RemoteIpAddress;
    public string Method => Context.Request.Method;
    public string Path => Context.Request.Path;
    public string Header(string name) => Context.Request.Headers[name].ToString();
    public IReadOnlyList<string> HeaderValues(string name) => Context.Request.Headers[name].Select(v => v!).ToArray();
}

public sealed class ResponseMetadata(IHttpContextAccessor accessor) : IResponseMetadata
{
    private IHeaderDictionary Headers => (accessor.HttpContext ?? throw new InvalidOperationException("No active request.")).Response.Headers;
    public string? this[string name]
    {
        get => Headers[name].ToString();
        set => Headers[name] = value;
    }
}

public static class IncomingContentAdapter
{
    public static IncomingContent Bind(HttpRequest request) => new BoundContent(request);

    // Preserve the original deferred request parsing (including failure timing).
    private sealed class BoundContent(HttpRequest request) : IncomingContent
    {
        public override Stream Body => request.Body;
        public override string? ContentType => request.ContentType;
        public override long? ContentLength => request.ContentLength;
        public override string? FirstHeader(string name) => request.Headers[name].FirstOrDefault();
        public override string? FirstQuery(string name) => request.Query[name].FirstOrDefault();
    }
}
