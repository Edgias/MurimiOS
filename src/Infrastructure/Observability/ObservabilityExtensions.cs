using EdgIQ.MurimiOS.Application.Common.Observability;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EdgIQ.MurimiOS.Infrastructure.Observability;
using PlatformDiagnostics = global::EdgIQ.MurimiOS.Application.Common.Observability.PlatformDiagnostics;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddMurimiOSObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment,
        string serviceName,
        bool instrumentAspNetCore)
    {
        string? endpoint = configuration["OpenTelemetry:OtlpEndpoint"];
        var telemetry = services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName, serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString())
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment.name"] = environment.EnvironmentName,
                    ["service.namespace"] = "MurimiOS"
                }))
            .WithTracing(tracing =>
            {
                tracing.AddSource(PlatformDiagnostics.ActivitySourceName, "Npgsql", "RabbitMQ.Client")
                    .AddHttpClientInstrumentation(options => options.RecordException = true);
                if (instrumentAspNetCore)
                    tracing.AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.Filter = context => !context.Request.Path.StartsWithSegments("/health/live");
                    });
                if (Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri))
                    tracing.AddOtlpExporter(options => options.Endpoint = uri);
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(PlatformDiagnostics.MeterName, "Npgsql", "RabbitMQ.Client")
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation();
                if (instrumentAspNetCore) metrics.AddAspNetCoreInstrumentation();
                if (Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri))
                    metrics.AddOtlpExporter(options => options.Endpoint = uri);
            });
        return services;
    }

    public static ILoggingBuilder AddMurimiOSOpenTelemetryLogging(
        this ILoggingBuilder logging,
        IConfiguration configuration)
    {
        string? endpoint = configuration["OpenTelemetry:OtlpEndpoint"];
        logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri))
                options.AddOtlpExporter(exporter => exporter.Endpoint = uri);
        });
        return logging;
    }
}
