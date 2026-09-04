using EdgIQ.MurimiOS.API.Infrastructure.Extensions;
using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.API.Infrastructure.Access;
using EdgIQ.MurimiOS.Infrastructure.Identity;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.API.Endpoints.Platform;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane;
using EdgIQ.MurimiOS.Application.Common.Reporting;
using EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using EdgIQ.MurimiOS.Domain.TenantMembership;


using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Imports;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Infrastructure.Observability;

using EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock;

using EdgIQ.MurimiOS.Infrastructure.Storage.Compliance;
using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;

using EdgIQ.MurimiOS.Application.Common.Finance;


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using System.Text.Json;

// Tenant routing changes connection strings before open, so spatial mappings must be available
// to every newly-created Npgsql connection rather than only to one fixed data source.
#pragma warning disable CS0618 // A fixed data source cannot support the connection-string routing interceptor.
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
#pragma warning restore CS0618

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMurimiOSApplication();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, EdgIQ.MurimiOS.API.Infrastructure.CurrentUser>();
builder.Services.AddScoped<IResponseMetadata, EdgIQ.MurimiOS.API.Infrastructure.ResponseMetadata>();
builder.Services.AddScoped<UseCaseContext>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddMurimiOSOpenTelemetryLogging(builder.Configuration);
builder.Services.AddMurimiOSObservability(
    builder.Configuration,
    builder.Environment,
    "MurimiOS.API",
    instrumentAspNetCore: true);

void ConfigurePostgres(DbContextOptionsBuilder options) =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("MurimiOS"),
        npgsql => npgsql.UseNetTopologySuite())
        .UseSnakeCaseNamingConvention();

builder.Services.AddScoped<HttpTenantContext>();
builder.Services.AddScoped<ITenantContext>(services => services.GetRequiredService<HttpTenantContext>());
builder.Services.AddScoped<RequestActorContext>();
builder.Services.AddScoped<IRequestActorContext>(services => services.GetRequiredService<RequestActorContext>());
builder.Services.AddScoped<IAuditActorContext>(services => services.GetRequiredService<RequestActorContext>());
builder.Services.AddScoped<OrganisationScopeAccess>();
builder.Services.AddScoped<IOrganisationScopeAccess>(services => services.GetRequiredService<OrganisationScopeAccess>());
builder.Services.AddScoped<IMutableTenantContext>(services => services.GetRequiredService<HttpTenantContext>());
builder.Services.AddSingleton<IIntegrationCredentialProtector>(services => services.GetRequiredService<IntegrationCredentialProtector>());
builder.Services.AddTransient<IIcarAdeExchangeAdapter>(services => services.GetRequiredService<IcarAdeExchangeAdapter>());
builder.Services.AddTransient<IScotEidMovementAdapter>(services => services.GetRequiredService<ScotEidMovementAdapter>());
builder.Services.AddScoped<IComplianceAuditPackGenerator>(services => services.GetRequiredService<ComplianceAuditPackGenerator>());
builder.Services.AddSingleton<IComplianceAuditPackSignatureService>(services => services.GetRequiredService<ComplianceAuditPackSignatureService>());
builder.Services.AddSingleton<IRequestQuotaStore>(services => services.GetRequiredService<RequestQuotaStore>());
builder.Services.AddSingleton<ITenantDatabaseRegistry>(services => services.GetRequiredService<TenantDatabaseRegistry>());
builder.Services.AddSingleton<ITenantDatabaseRouteResolver>(services => services.GetRequiredService<TenantDatabaseRouteResolver>());
builder.Services.AddScoped<TenantConnectionInterceptor>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddDbContext<PlatformDbContext>((services, options) =>
{
    ConfigurePostgres(options);
    options.AddInterceptors(
        services.GetRequiredService<TenantConnectionInterceptor>(),
        services.GetRequiredService<AuditSaveChangesInterceptor>());
});
builder.Services.AddScoped<EdgIQ.MurimiOS.Application.Sync.PostSyncPush.SyncCommandProcessor>();
builder.Services.AddSingleton<ProductionReportPdfBuilder>();
builder.Services.AddScoped<EnterpriseConsolidationEngine>();
builder.Services.AddSingleton(TimeProvider.System);
var identitySecurityOptions = new IdentitySecurityOptions();
builder.Configuration.GetSection("Authentication:Security").Bind(identitySecurityOptions);
identitySecurityOptions.Validate(builder.Environment, builder.Configuration);
builder.Services.AddSingleton(identitySecurityOptions);
builder.Services.AddMurimiOSQuotaProtection(builder.Configuration);
builder.Services.AddSingleton<GeospatialImportParser>();
builder.Services.AddMurimiOSCompliance(builder.Configuration);
var (scannerProvider, clamAvOptions) = builder.Services.AddMurimiOSMediaStorage(builder.Configuration, builder.Environment);
builder.Services.AddMurimiOSLivestockAdapters();
string platformConnection = builder.Configuration.GetConnectionString("MurimiOS")
    ?? throw new InvalidOperationException("ConnectionStrings:MurimiOS is required.");
builder.Services.AddMurimiOSTenantRouting(builder.Configuration, platformConnection);
var rabbitOptions = new RabbitMqPublisherOptions();
builder.Configuration.GetSection("RabbitMq").Bind(rabbitOptions);
IHealthChecksBuilder healthChecks = builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddCheck("postgres", new PostgresHealthCheck(platformConnection), failureStatus: HealthStatus.Unhealthy, tags: ["ready"])
    .AddCheck("outbox", new OutboxBacklogHealthCheck(platformConnection, TimeProvider.System, TimeSpan.FromMinutes(5)), failureStatus: HealthStatus.Degraded, tags: ["ready"])
    .AddCheck("rabbitmq", new RabbitMqHealthCheck(rabbitOptions), failureStatus: HealthStatus.Degraded, tags: ["ready"])
    .AddCheck<MediaStorageHealthCheck>("media-storage", failureStatus: HealthStatus.Degraded, tags: ["ready"]);
if (scannerProvider.Equals("ClamAv", StringComparison.OrdinalIgnoreCase))
    healthChecks.AddCheck("clamav", new ClamAvHealthCheck(clamAvOptions), failureStatus: HealthStatus.Degraded, tags: ["ready"]);

const string authenticationSelector = "MurimiOSAuthentication";
bool developmentHeaderEnabled = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue("Authentication:AllowDevelopmentTenantHeader", false);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = authenticationSelector;
        options.DefaultChallengeScheme = authenticationSelector;
    })
    .AddPolicyScheme(authenticationSelector, authenticationSelector, options =>
    {
        options.ForwardDefaultSelector = context => developmentHeaderEnabled
            && !context.Request.Headers.Authorization.ToString().StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? DevelopmentHeaderAuthenticationHandler.SchemeName
                : JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        string? authority = builder.Configuration["Authentication:Authority"];
        if (!string.IsNullOrWhiteSpace(authority)) options.Authority = authority;
        options.Audience = builder.Configuration["Authentication:Audience"];
        options.RequireHttpsMetadata = builder.Configuration.GetValue("Authentication:RequireHttpsMetadata", true);
        options.MapInboundClaims = false;
    })
    .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevelopmentHeaderAuthenticationHandler>(
        DevelopmentHeaderAuthenticationHandler.SchemeName,
        _ => { });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    foreach (string permission in PlatformPermissions.All)
        options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
    options.AddPolicy(SaasControlPlaneEndpoints.PlatformOperatorPolicy,
        policy => policy.RequireClaim("platform_operator", "true"));
});

bool legacyEndpointsEnabled = builder.Configuration.GetValue("Features:LegacyEndpointsEnabled", false);
if (legacyEndpointsEnabled)
{
    builder.Services.AddDbContext<MurimiOSDbContext>(ConfigurePostgres);
}

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(EdgIQ.MurimiOS.API.Infrastructure.PublishedSchemaIds.Select));
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:4200"])
        .AllowAnyHeader()
        .AllowAnyMethod()));

WebApplication app = builder.Build();

var liveHealthOptions = new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponseAsync
};
var readyHealthOptions = new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponseAsync
};
app.MapHealthChecks("/health/live", liveHealthOptions).AllowAnonymous().ExcludeFromDescription();
app.MapHealthChecks("/health/ready", readyHealthOptions).AllowAnonymous().ExcludeFromDescription();
app.MapHealthChecks("/health", readyHealthOptions).AllowAnonymous().ExcludeFromDescription();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<RequestQuotaMiddleware>();
app.UseMiddleware<IdentitySecurityMiddleware>();
app.UseMiddleware<SaasTenantLifecycleMiddleware>();
app.UseMiddleware<TenantMigrationWriteGuardMiddleware>();
app.UseMiddleware<RequestTelemetryMiddleware>();
app.UseAuthorization();

app.MapPlatformEndpoints();
if (legacyEndpointsEnabled)
{
    app.RegisterEndpoints();
}

app.Run();

static Task WriteHealthResponseAsync(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsync(JsonSerializer.Serialize(new
    {
        status = report.Status.ToString().ToLowerInvariant(),
        durationMilliseconds = Math.Round(report.TotalDuration.TotalMilliseconds, 2),
        checks = report.Entries.OrderBy(x => x.Key).Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString().ToLowerInvariant(),
            description = entry.Value.Description,
            durationMilliseconds = Math.Round(entry.Value.Duration.TotalMilliseconds, 2),
            data = entry.Value.Data
        })
    }));
}

public partial class Program;
