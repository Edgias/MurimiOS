using System.Collections.Concurrent;
using System.Buffers;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Infrastructure.Integrations;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using MQTTnet;
using MQTTnet.Formatter;
using MQTTnet.Protocol;

namespace EdgIQ.MurimiOS.Worker;
using MqttSubscriptionDescriptor = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttSubscriptionDescriptor;
using MqttSubscriptionConfiguration = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttSubscriptionConfiguration;
using IMqttSubscriptionStore = global::EdgIQ.MurimiOS.Infrastructure.Integrations.IMqttSubscriptionStore;

public sealed class MqttSubscriptionWorker(IMqttSubscriptionStore store, MqttSubscriptionLoader loader,
    ProviderCredentialReader credentials, MqttMessageProcessor processor, ILogger<MqttSubscriptionWorker> logger) : BackgroundService
{
    private sealed record Session(MqttSubscriptionDescriptor Descriptor, CancellationTokenSource Stop, Task Task);
    private readonly ConcurrentDictionary<Guid, Session> sessions = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                IReadOnlyList<MqttSubscriptionDescriptor> active = await store.ListActiveAsync(stoppingToken);
                var activeIds = active.Select(x => x.FeedId).ToHashSet();
                foreach ((Guid feedId, Session session) in sessions.Where(x => !activeIds.Contains(x.Key)).ToArray())
                    if (sessions.TryRemove(feedId, out _)) session.Stop.Cancel();
                foreach (MqttSubscriptionDescriptor descriptor in active)
                {
                    if (sessions.TryGetValue(descriptor.FeedId, out Session? current) && current.Descriptor == descriptor) continue;
                    if (current is not null && sessions.TryRemove(descriptor.FeedId, out _)) current.Stop.Cancel();
                    var stop = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    Task task = RunSubscriptionAsync(descriptor, stop.Token);
                    sessions[descriptor.FeedId] = new Session(descriptor, stop, task);
                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogError(exception, "MQTT subscription discovery failed.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        foreach (Session session in sessions.Values) session.Stop.Cancel();
        await Task.WhenAll(sessions.Values.Select(x => x.Task).ToArray());
    }

    private async Task RunSubscriptionAsync(MqttSubscriptionDescriptor descriptor, CancellationToken cancellationToken)
    {
        int failures = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                MqttSubscriptionConfiguration configuration = await loader.LoadAsync(descriptor, cancellationToken);
                await RunConnectedAsync(configuration, cancellationToken);
                failures = 0;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                failures++;
                int delay = Math.Min(60, 2 << Math.Min(5, failures - 1));
                logger.LogWarning(exception, "MQTT feed {FeedId} disconnected; reconnecting in {DelaySeconds}s.", descriptor.FeedId, delay);
                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken);
            }
        }
    }

    private async Task RunConnectedAsync(MqttSubscriptionConfiguration subscription, CancellationToken cancellationToken)
    {
        MqttSubscriptionDescriptor descriptor = subscription.Descriptor;
        JsonElement connection = Object(subscription.ConnectionConfigurationJson, "connection configuration");
        JsonElement feed = Object(subscription.FeedConfigurationJson, "feed configuration");
        JsonElement secret = Object(credentials.Unprotect(subscription.ProtectedCredentialReference), "credential");
        string host = String(connection, "host") ?? throw new InvalidOperationException("MQTT host is required.");
        bool tls = Boolean(connection, "tls", true);
        int port = Integer(connection, "port", tls ? 8883 : 1883);
        string topic = String(feed, "topic") ?? throw new InvalidOperationException("MQTT topic filter is required.");
        bool thingsStack = subscription.ProviderCode.Equals("the-things-stack", StringComparison.OrdinalIgnoreCase);
        uint sessionExpiry = (uint)Math.Clamp(Integer(connection, "sessionExpirySeconds", 86_400), 60, 2_592_000);
        string clientId = $"{String(connection, "clientIdPrefix") ?? "murimios"}-{descriptor.FeedId:N}";
        if (clientId.Length > 120) clientId = clientId[..120];
        var factory = new MqttClientFactory();
        using IMqttClient client = factory.CreateMqttClient();
        var disconnected = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        client.DisconnectedAsync += _ => { disconnected.TrySetResult(); return Task.CompletedTask; };
        client.ApplicationMessageReceivedAsync += async args =>
        {
            try
            {
                string payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload.ToArray());
                await processor.ProcessAsync(subscription, args.ApplicationMessage.Topic, payload, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "MQTT message processing failed for feed {FeedId} and topic {Topic}.", descriptor.FeedId, args.ApplicationMessage.Topic);
            }
        };
        MqttClientOptionsBuilder options = factory.CreateClientOptionsBuilder().WithTcpServer(host, port)
            .WithClientId(clientId).WithKeepAlivePeriod(TimeSpan.FromSeconds(Math.Clamp(Integer(connection, "keepAliveSeconds", 30), 5, 300)))
            .WithTimeout(TimeSpan.FromSeconds(20));
        if (thingsStack) options.WithProtocolVersion(MqttProtocolVersion.V311).WithCleanSession(true);
        else options.WithProtocolVersion(MqttProtocolVersion.V500).WithCleanStart(false).WithSessionExpiryInterval(sessionExpiry);
        string? username = String(secret, "username"); string? password = String(secret, "password") ?? String(secret, "apiKey");
        if (!string.IsNullOrWhiteSpace(username)) options.WithCredentials(username, password ?? string.Empty);
        if (tls) options.WithTlsOptions(value => value.UseTls().WithTargetHost(host));
        await client.ConnectAsync(options.Build(), cancellationToken);
        MqttQualityOfServiceLevel qos = thingsStack ? MqttQualityOfServiceLevel.AtMostOnce
            : (MqttQualityOfServiceLevel)Math.Clamp(Integer(feed, "qos", 1), 0, 2);
        await client.SubscribeAsync(factory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(topic, qos, false, false, MqttRetainHandling.SendAtSubscribe).Build(), cancellationToken);
        logger.LogInformation("MQTT feed {FeedId} subscribed to {Topic} on {Host}:{Port}.", descriptor.FeedId, topic, host, port);
        using CancellationTokenRegistration registration = cancellationToken.Register(() => disconnected.TrySetCanceled(cancellationToken));
        try { await disconnected.Task; }
        finally
        {
            if (client.IsConnected) await client.DisconnectAsync(factory.CreateClientDisconnectOptionsBuilder().Build(), CancellationToken.None);
        }
        if (!cancellationToken.IsCancellationRequested) throw new IOException("MQTT broker disconnected the subscription.");
    }

    private static JsonElement Object(string json, string name)
    {
        try { JsonElement value = JsonSerializer.Deserialize<JsonElement>(json); return value.ValueKind == JsonValueKind.Object ? value : throw new JsonException(); }
        catch (JsonException exception) { throw new InvalidOperationException($"MQTT {name} must be a JSON object.", exception); }
    }
    private static string? String(JsonElement value, string name) => value.TryGetProperty(name, out JsonElement property) && property.ValueKind == JsonValueKind.String ? property.GetString() : null;
    private static int Integer(JsonElement value, string name, int fallback) => value.TryGetProperty(name, out JsonElement property) && property.TryGetInt32(out int result) ? result : fallback;
    private static bool Boolean(JsonElement value, string name, bool fallback) => value.TryGetProperty(name, out JsonElement property) && property.ValueKind is JsonValueKind.True or JsonValueKind.False ? property.GetBoolean() : fallback;
}
