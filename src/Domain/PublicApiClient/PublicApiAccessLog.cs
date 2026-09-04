using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.PublicApiClient;

public sealed class PublicApiAccessLog : TenantEntity
{
    private PublicApiAccessLog() { }
    public PublicApiAccessLog(Guid id, Guid clientId, string scope, string method, string path, int statusCode,
        string correlationId, DateTimeOffset occurredAt, string? remoteAddressHash) : base(id)
    { if (clientId == Guid.Empty || statusCode is < 100 or > 599) throw new ArgumentException("Client and HTTP status are required."); ClientId = clientId; Scope = PublicApiClient.Text(scope, 80); Method = PublicApiClient.Text(method, 12); Path = PublicApiClient.Text(path, 500); StatusCode = statusCode; CorrelationId = PublicApiClient.Text(correlationId, 160); OccurredAt = occurredAt.ToUniversalTime(); RemoteAddressHash = remoteAddressHash is null ? null : PublicApiClient.Hash(remoteAddressHash); }
    public Guid ClientId { get; private set; } public string Scope { get; private set; } = ""; public string Method { get; private set; } = ""; public string Path { get; private set; } = ""; public int StatusCode { get; private set; } public string CorrelationId { get; private set; } = ""; public DateTimeOffset OccurredAt { get; private set; } public string? RemoteAddressHash { get; private set; }
}
