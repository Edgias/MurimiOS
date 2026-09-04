using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface IRequestQuotaStore
{
    Task<RequestQuotaDecision> ConsumeTenantAsync(Guid tenantId, DateTimeOffset now, CancellationToken ct);
    Task<RequestQuotaDecision> ConsumeClientAsync(Guid tenantId, Guid clientId, DateTimeOffset now, CancellationToken ct);
}
