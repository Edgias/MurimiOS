using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.API.Infrastructure.Access;

public sealed class DevelopmentHeaderAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration,
    IWebHostEnvironment environment)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "DevelopmentHeader";
    public const string AuthenticationMethod = "development_header";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!environment.IsDevelopment() || !configuration.GetValue("Authentication:AllowDevelopmentTenantHeader", false))
            return Task.FromResult(AuthenticateResult.NoResult());

        string? tenant = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!Guid.TryParse(tenant, out Guid tenantId) || tenantId == Guid.Empty)
            return Task.FromResult(AuthenticateResult.NoResult());

        string subject = Request.Headers["X-Development-Subject"].FirstOrDefault()
            ?? configuration["Authentication:DevelopmentSubject"]
            ?? "development-bootstrap";
        var claims = new List<Claim>
        {
            new("sub", subject),
            new(ClaimTypes.NameIdentifier, subject),
            new(ClaimTypes.Name, subject),
            new("tenant_id", tenantId.ToString()),
            new("auth_method", AuthenticationMethod)
        };
        if (configuration.GetValue("Authentication:DevelopmentPlatformOperator", false))
            claims.Add(new Claim("platform_operator", "true"));
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
