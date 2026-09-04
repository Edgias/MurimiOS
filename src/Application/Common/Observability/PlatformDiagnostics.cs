using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace EdgIQ.MurimiOS.Application.Common.Observability;

public static class PlatformDiagnostics
{
    public const string ActivitySourceName = "MurimiOS.Platform";
    public const string MeterName = "MurimiOS.Platform";

    public static readonly ActivitySource Activities = new(ActivitySourceName);
    public static readonly Meter Meter = new(MeterName);
    public static readonly Counter<long> OutboxPublished = Meter.CreateCounter<long>(
        "murimios.outbox.published", unit: "{message}", description: "Broker-confirmed integration events.");
    public static readonly Counter<long> OutboxFailures = Meter.CreateCounter<long>(
        "murimios.outbox.publish_failures", unit: "{message}", description: "Failed outbox publish attempts.");
    public static readonly Histogram<double> OutboxLagSeconds = Meter.CreateHistogram<double>(
        "murimios.outbox.lag", unit: "s", description: "Time from event occurrence to broker confirmation.");
    public static readonly Counter<long> MediaCommitted = Meter.CreateCounter<long>(
        "murimios.media.committed", unit: "{object}", description: "Validated media objects committed to durable storage.");
    public static readonly Counter<long> MediaScanFailures = Meter.CreateCounter<long>(
        "murimios.media.scan_failures", unit: "{scan}", description: "Unavailable or threat-positive media scans.");
}
