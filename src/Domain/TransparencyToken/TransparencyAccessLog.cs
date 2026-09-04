using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.TransparencyToken;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class TransparencyAccessLog : TenantEntity
{
    private TransparencyAccessLog() { }
    public TransparencyAccessLog(Guid id, Guid tokenId, DateTimeOffset occurredAt, string outcome, string correlationId, string? remoteAddressHash, string? userAgentHash) : base(id)
    { if (tokenId == Guid.Empty) throw new ArgumentException("Token is required."); TokenId = tokenId; OccurredAt = occurredAt.ToUniversalTime(); Outcome = PublicApiClient.Text(outcome, 40); CorrelationId = PublicApiClient.Text(correlationId, 160); RemoteAddressHash = remoteAddressHash is null ? null : PublicApiClient.Hash(remoteAddressHash); UserAgentHash = userAgentHash is null ? null : PublicApiClient.Hash(userAgentHash); }
    public Guid TokenId { get; private set; } public DateTimeOffset OccurredAt { get; private set; } public string Outcome { get; private set; } = ""; public string CorrelationId { get; private set; } = ""; public string? RemoteAddressHash { get; private set; } public string? UserAgentHash { get; private set; }
}
