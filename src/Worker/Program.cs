using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Infrastructure.DependencyInjection;
using EdgIQ.MurimiOS.Worker;
using EdgIQ.MurimiOS.Infrastructure.Observability;
using EdgIQ.MurimiOS.Application.Common.Observability;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Integrations;

using EdgIQ.MurimiOS.Domain.Common;



using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;

using EdgIQ.MurimiOS.Infrastructure.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

// Worker jobs can open routed tenant databases; register PostGIS serializers process-wide.
#pragma warning disable CS0618 // A fixed data source cannot support per-job tenant database routing.
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
#pragma warning restore CS0618

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationPersistenceContracts();
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ ");
builder.Logging.AddMurimiOSOpenTelemetryLogging(builder.Configuration);
builder.Services.AddMurimiOSObservability(
    builder.Configuration,
    builder.Environment,
    "MurimiOS.Worker",
    instrumentAspNetCore: false);

builder.Services.AddOptions<OutboxDispatcherOptions>()
    .Bind(builder.Configuration.GetSection("Outbox"))
    .Validate(options => IsValid(options), "Outbox configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddOptions<RabbitMqPublisherOptions>()
    .Bind(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddOptions<WebhookDispatcherOptions>()
    .Bind(builder.Configuration.GetSection("Webhooks"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Webhook configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddOptions<ProviderIngestionOptions>()
    .Bind(builder.Configuration.GetSection("ProviderIngestion"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Provider ingestion configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddOptions<TenantMigrationExecutionOptions>()
    .Bind(builder.Configuration.GetSection("TenantMigrationExecution"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Tenant migration execution configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddOptions<AnalyticsMaintenanceOptions>()
    .Bind(builder.Configuration.GetSection("AnalyticsMaintenance"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Analytics maintenance configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddOptions<ConnectedDataLifecycleOptions>()
    .Bind(builder.Configuration.GetSection("ConnectedDataLifecycle"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Connected-data lifecycle configuration is invalid.")
    .ValidateOnStart();

builder.Services.AddSingleton(TimeProvider.System);
string workerConnection = builder.Configuration.GetConnectionString("MurimiOSWorker")
    ?? builder.Configuration.GetConnectionString("MurimiOS")
    ?? throw new InvalidOperationException("ConnectionStrings:MurimiOSWorker is required.");
builder.Services.AddOptions<TenantDatabaseOptions>()
    .Bind(builder.Configuration.GetSection("TenantDatabases"))
    .Validate(options => { try { options.Validate(); return true; } catch { return false; } }, "Tenant database configuration is invalid.")
    .ValidateOnStart();
builder.Services.AddSingleton(services => new TenantDatabaseRegistry(
    services.GetRequiredService<IOptions<TenantDatabaseOptions>>(),
    services.GetRequiredService<IConfiguration>(), workerConnection));
builder.Services.AddSingleton<TenantDatabaseRouteResolver>();
builder.Services.AddScoped<TenantDatabaseRoutingScope>();
builder.Services.AddScoped<WorkerTenantContext>();
builder.Services.AddScoped<ITenantContext>(services => services.GetRequiredService<WorkerTenantContext>());
builder.Services.AddScoped<TenantConnectionInterceptor>();
builder.Services.AddDbContext<PlatformDbContext>((services, options) => options
    .UseNpgsql(workerConnection, npgsql => npgsql.UseNetTopologySuite())
    .UseSnakeCaseNamingConvention()
    .AddInterceptors(services.GetRequiredService<TenantConnectionInterceptor>()));
string configuredKeyRoot = builder.Configuration["Storage:DataProtectionKeyRoot"] ?? "App_Data/keys";
string keyRoot = Path.IsPathRooted(configuredKeyRoot) ? configuredKeyRoot : Path.Combine(builder.Environment.ContentRootPath, configuredKeyRoot);
builder.Services.AddDataProtection().SetApplicationName("MurimiOS").PersistKeysToFileSystem(new DirectoryInfo(keyRoot));
string configuredMediaRoot = builder.Configuration["Storage:MediaRoot"] ?? "App_Data/media";
string mediaRoot = Path.IsPathRooted(configuredMediaRoot)
    ? configuredMediaRoot
    : Path.Combine(builder.Environment.ContentRootPath, configuredMediaRoot);
string storageProvider = builder.Configuration["Storage:Provider"] ?? "Local";
if (storageProvider.Equals("S3", StringComparison.OrdinalIgnoreCase))
{
    var s3Options = new S3MediaStorageOptions();
    builder.Configuration.GetSection("Storage:S3").Bind(s3Options);
    s3Options.StagingRoot = Path.IsPathRooted(s3Options.StagingRoot)
        ? s3Options.StagingRoot
        : Path.Combine(builder.Environment.ContentRootPath, s3Options.StagingRoot);
    builder.Services.AddSingleton<IMediaStorage>(new S3MediaStorage(s3Options));
}
else if (storageProvider.Equals("Local", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IMediaStorage>(new LocalMediaStorage(mediaRoot));
}
else
{
    throw new InvalidOperationException($"Unsupported Storage:Provider '{storageProvider}'.");
}
builder.Services.AddSingleton<ProviderCredentialReader>();
builder.Services.AddSingleton<IScheduledIngestionStore>(new PostgresScheduledIngestionStore(workerConnection));
builder.Services.AddSingleton<IMqttSubscriptionStore>(new PostgresMqttSubscriptionStore(workerConnection));
builder.Services.AddHttpClient<OpenMeteoWeatherAdapter>(client => client.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddHttpClient<SentinelHubSatelliteAdapter>(client => client.Timeout = TimeSpan.FromSeconds(120));
builder.Services.AddHttpClient<EuPesticidesRegulatoryAdapter>(client => client.Timeout = TimeSpan.FromMinutes(4));
builder.Services.AddHttpClient<CanonicalMachineryRestAdapter>(client => client.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddHttpClient<CanonicalErpRestAdapter>(client => client.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddHttpClient<CanonicalAccountingRestAdapter>(client => client.Timeout = TimeSpan.FromSeconds(60));
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<OpenMeteoWeatherAdapter>());
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<SentinelHubSatelliteAdapter>());
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<EuPesticidesRegulatoryAdapter>());
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<CanonicalMachineryRestAdapter>());
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<CanonicalErpRestAdapter>());
builder.Services.AddScoped<IProviderAdapter>(services => services.GetRequiredService<CanonicalAccountingRestAdapter>());
builder.Services.AddHostedService<ProviderIngestionWorker>();
builder.Services.AddSingleton<MqttMessageProcessor>();
builder.Services.AddSingleton<MqttSubscriptionLoader>();
builder.Services.AddHostedService<MqttSubscriptionWorker>();
builder.Services.AddSingleton<ITenantMigrationControlStore, PostgresTenantMigrationControlStore>();
builder.Services.AddSingleton<TenantMigrationArtifactService>();
builder.Services.AddHostedService<TenantMigrationWorker>();
builder.Services.AddSingleton<AnalyticsProjectionEngine>();
builder.Services.AddSingleton<AnalyticsDimensionSnapshotBuilder>();
builder.Services.AddSingleton<AnalyticsForecastEvaluator>();
builder.Services.AddSingleton<IAnalyticsMaintenanceStore>(new PostgresAnalyticsMaintenanceStore(workerConnection));
builder.Services.AddHostedService<AnalyticsMaintenanceWorker>();
builder.Services.AddSingleton<IConnectedDataLifecycleStore>(new PostgresConnectedDataLifecycleStore(workerConnection));
builder.Services.AddHostedService<ConnectedDataLifecycleWorker>();
builder.Services.AddSingleton<IOutboxStore>(services =>
{
    IConfiguration configuration = services.GetRequiredService<IConfiguration>();
    string connectionString = configuration.GetConnectionString("MurimiOSWorker")
        ?? configuration.GetConnectionString("MurimiOS")
        ?? throw new InvalidOperationException("ConnectionStrings:MurimiOSWorker is required.");
    return new PostgresOutboxStore(connectionString);
});

string publisher = builder.Configuration["Messaging:Publisher"] ?? "Logging";
if (publisher.Equals("RabbitMq", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IIntegrationEventPublisher>(services =>
        new RabbitMqIntegrationEventPublisher(
            services.GetRequiredService<IOptions<RabbitMqPublisherOptions>>().Value));
}
else if (publisher.Equals("Logging", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IIntegrationEventPublisher, LoggingIntegrationEventPublisher>();
}
else
{
    throw new InvalidOperationException($"Unsupported Messaging:Publisher '{publisher}'.");
}

builder.Services.AddSingleton(services => new OutboxDispatcher(
    services.GetRequiredService<IOutboxStore>(),
    services.GetRequiredService<IIntegrationEventPublisher>(),
    services.GetRequiredService<IOptions<OutboxDispatcherOptions>>().Value,
    services.GetRequiredService<TimeProvider>(),
    services.GetRequiredService<ILogger<OutboxDispatcher>>()));
builder.Services.AddHostedService<OutboxWorker>();
builder.Services.AddSingleton<IWebhookStore>(services =>
{
    IConfiguration configuration = services.GetRequiredService<IConfiguration>();
    string connectionString = configuration.GetConnectionString("MurimiOSWorker") ?? configuration.GetConnectionString("MurimiOS")
        ?? throw new InvalidOperationException("ConnectionStrings:MurimiOSWorker is required.");
    return new PostgresWebhookStore(connectionString);
});
builder.Services.AddSingleton(services =>
{
    WebhookDispatcherOptions options = services.GetRequiredService<IOptions<WebhookDispatcherOptions>>().Value;
    return new HttpClient(new SocketsHttpHandler { AllowAutoRedirect = false, UseCookies = false }) { Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds) };
});
builder.Services.AddSingleton(services => new WebhookDispatcher(services.GetRequiredService<IWebhookStore>(), services.GetRequiredService<HttpClient>(), services.GetRequiredService<IOptions<WebhookDispatcherOptions>>().Value, services.GetRequiredService<TimeProvider>(), services.GetRequiredService<ILogger<WebhookDispatcher>>()));
builder.Services.AddHostedService<WebhookWorker>();

await builder.Build().RunAsync();

static bool IsValid(OutboxDispatcherOptions options)
{
    try
    {
        options.Validate();
        return true;
    }
    catch (InvalidOperationException)
    {
        return false;
    }
}
