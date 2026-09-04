using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Access.GetAccessMe;

public sealed class GetAccessMeHandler(IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<GetAccessMeQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetAccessMeQuery message, CancellationToken cancellationToken) => Task.FromResult(ExecuteAsync(actor, tenant));
private static UseCaseResult ExecuteAsync(IRequestActorContext actor, ITenantContext tenant) =>UseCaseResults.Ok(new
        {
            tenantId = tenant.TenantId,
            actor.Subject,
            actor.MembershipId,
            role = actor.Role.ToString(),
            actor.ScopeOrganisationNodeId,
            permissions = actor.Permissions.Order(StringComparer.Ordinal)
        });

}
