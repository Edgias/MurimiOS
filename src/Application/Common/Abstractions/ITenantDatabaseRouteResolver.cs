using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
public interface ITenantDatabaseRouteResolver
{
    void Invalidate(Guid tenantId);
    Task<bool> IsWriteBlockedAsync(Guid tenantId, CancellationToken cancellationToken);
}
