using System.Net;
using System.Security.Claims;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface ICurrentUser
{
    ClaimsPrincipal Principal { get; }
    string CorrelationId { get; }
    IPAddress? RemoteAddress { get; }
    string Method { get; }
    string Path { get; }
    string Header(string name);
    IReadOnlyList<string> HeaderValues(string name);
}

internal static class CurrentUserClaimsExtensions
{
    public static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) => principal.FindFirst(claimType)?.Value;
}
