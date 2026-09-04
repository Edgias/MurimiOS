using System.Security.Claims;
using EdgIQ.MurimiOS.API.Infrastructure.Access;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using IPlatformDbContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext;
using DevelopmentHeaderAuthenticationHandler = global::EdgIQ.MurimiOS.API.Infrastructure.Access.DevelopmentHeaderAuthenticationHandler;
using RequestActorContext = global::EdgIQ.MurimiOS.API.Infrastructure.Access.RequestActorContext;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        HttpTenantContext tenantContext,
        RequestActorContext actorContext,
        IPlatformDbContext db,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        if (IsPublicPath(context.Request.Path))
        {
            await next(context);
            return;
        }

        if (context.User.Identity?.IsAuthenticated != true)
        {
            await ProblemAsync(context, StatusCodes.Status401Unauthorized, "authentication_required", "Authentication required", "Supply a valid bearer token.");
            return;
        }

        string? claimTenant = context.User.FindFirst("tenant_id")?.Value;
        string? headerTenant = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (Guid.TryParse(claimTenant, out Guid claimedId)
            && Guid.TryParse(headerTenant, out Guid headerId)
            && claimedId != headerId)
        {
            await ProblemAsync(context, StatusCodes.Status403Forbidden, "tenant_mismatch", "Tenant selection rejected", "The bearer token and tenant header identify different tenants.");
            return;
        }

        string? tenantValue = claimTenant ?? headerTenant;
        if (!Guid.TryParse(tenantValue, out Guid tenantId) || tenantId == Guid.Empty)
        {
            await ProblemAsync(context, StatusCodes.Status400BadRequest, "tenant_required", "Tenant context required", "Select a tenant using an authenticated tenant_id claim or X-Tenant-Id header.");
            return;
        }

        string? subject = context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(subject))
        {
            await ProblemAsync(context, StatusCodes.Status401Unauthorized, "subject_required", "Invalid identity", "The authenticated identity has no stable subject claim.");
            return;
        }

        tenantContext.Resolve(tenantId);
        bool developmentBootstrap = environment.IsDevelopment()
            && configuration.GetValue("Authentication:AllowDevelopmentTenantHeader", false)
            && context.User.HasClaim("auth_method", DevelopmentHeaderAuthenticationHandler.AuthenticationMethod)
            && string.Equals(
                subject,
                configuration["Authentication:DevelopmentSubject"] ?? "development-bootstrap",
                StringComparison.Ordinal);

        TenantMembership? membership = await db.TenantMemberships
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.ExternalSubject == subject && x.Status == MembershipStatus.Active, context.RequestAborted);
        if (membership is null)
        {
            if (developmentBootstrap)
            {
                actorContext.Resolve(subject, null, TenantRole.Owner, null, PlatformPermissions.All);
            }
            else
            {
                await ProblemAsync(context, StatusCodes.Status403Forbidden, "membership_required", "Tenant access denied", "No active membership exists for this identity and tenant.");
                return;
            }
        }
        else
        {
            string[] explicitGrants = await db.MembershipPermissionGrants
                .AsNoTracking()
                .Where(x => x.MembershipId == membership.Id)
                .Select(x => x.Permission)
                .ToArrayAsync(context.RequestAborted);
            string[] permissions = PlatformPermissions.ForRole(membership.Role)
                .Concat(explicitGrants)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            actorContext.Resolve(subject, membership.Id, membership.Role, membership.ScopeOrganisationNodeId, permissions);
        }

        if (context.User.Identity is ClaimsIdentity identity)
        {
            AddClaimIfMissing(identity, "tenant_id", tenantId.ToString());
            if (actorContext.MembershipId is { } membershipId) AddClaimIfMissing(identity, "membership_id", membershipId.ToString());
            AddClaimIfMissing(identity, ClaimTypes.Role, actorContext.Role.ToString());
            foreach (string permission in actorContext.Permissions) AddClaimIfMissing(identity, "permission", permission);
            if (actorContext.ScopeOrganisationNodeId is { } scopeId) AddClaimIfMissing(identity, "organisation_scope_id", scopeId.ToString());
        }

        context.Response.Headers["X-Tenant-Id"] = tenantId.ToString();
        await next(context);
    }

    private static void AddClaimIfMissing(ClaimsIdentity identity, string type, string value)
    {
        if (!identity.HasClaim(type, value)) identity.AddClaim(new Claim(type, value));
    }

    private static Task ProblemAsync(HttpContext context, int status, string code, string title, string detail)
    {
        context.Response.StatusCode = status;
        return Results.Problem(
            statusCode: status,
            title: title,
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code })
            .ExecuteAsync(context);
    }

    private static bool IsPublicPath(PathString path) =>
        path.StartsWithSegments("/health")
        || path.StartsWithSegments("/swagger")
        || path.StartsWithSegments("/openapi")
        || path.StartsWithSegments("/api/public/v1");
}
