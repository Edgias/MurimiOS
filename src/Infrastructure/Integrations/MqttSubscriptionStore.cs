using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;

public sealed record MqttSubscriptionDescriptor(Guid TenantId, Guid ConnectionId, Guid FeedId, Guid FarmId,
    uint ConnectionVersion, uint FeedVersion, Guid CredentialId);

public sealed record MqttSubscriptionConfiguration(MqttSubscriptionDescriptor Descriptor, string ProviderCode,
    string ConnectionConfigurationJson, string FeedConfigurationJson, string ProtectedCredentialReference);

public interface IMqttSubscriptionStore
{
    Task<IReadOnlyList<MqttSubscriptionDescriptor>> ListActiveAsync(CancellationToken cancellationToken);
}

public sealed class PostgresMqttSubscriptionStore(string connectionString) : IMqttSubscriptionStore
{
    public async Task<IReadOnlyList<MqttSubscriptionDescriptor>> ListActiveAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("select tenant_id,connection_id,feed_id,farm_id,connection_version,feed_version,credential_id from integrations.list_mqtt_subscriptions()", connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        var rows = new List<MqttSubscriptionDescriptor>();
        while (await reader.ReadAsync(cancellationToken))
            rows.Add(new MqttSubscriptionDescriptor(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2), reader.GetGuid(3),
                checked((uint)reader.GetInt64(4)), checked((uint)reader.GetInt64(5)), reader.GetGuid(6)));
        return rows;
    }
}
