using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateDisclosurePolicy;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;

public sealed class CreateDisclosurePolicyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<CreateDisclosurePolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateDisclosurePolicyCommand message, CancellationToken cancellationToken) => CreateDisclosurePolicyAsync(message.request, db, scope, actor, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateDisclosurePolicyAsync(CreateDisclosurePolicyRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, CancellationToken ct)
    {
        if (request.DataGrantId.HasValue && request.FarmIds.Count > 0) return Bad("policy_scope_invalid", "Contracted-production policies use the data grant's farm scope; local farm ids must be empty.");
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        if (request.FarmIds.Count > 0 && (!resolved.IsUnrestricted && !resolved.FarmIds!.IsSupersetOf(request.FarmIds)
            || resolved.IsUnrestricted && await db.Farms.CountAsync(x => request.FarmIds.Contains(x.Id), ct) != request.FarmIds.Distinct().Count())) return Bad("farm_scope_invalid", "Every local disclosure farm must be accessible.");
        RetailDisclosurePolicy? previous = request.PreviousPolicyId.HasValue ? await db.RetailDisclosurePolicies.SingleOrDefaultAsync(x => x.Id == request.PreviousPolicyId, ct) : null;
        if (request.PreviousPolicyId.HasValue && previous is null) return Bad("policy_lineage_invalid", "Previous policy does not exist.");
        if (request.DataGrantId.HasValue)
        {
            DataGrant? grant = await db.DataGrants.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.DataGrantId && x.RecipientTenantId == tenant.TenantId, ct);
            if (grant is null || !grant.DownstreamDisclosureAllowed || grant.Status != DataGrantStatus.Active) return Bad("grant_disclosure_invalid", "An active downstream-disclosure grant is required.");
        }
        try { Guid id = Id(request.Id); var policy = new RetailDisclosurePolicy(id, previous?.SeriesId ?? id, request.Name, request.PublicBrandName, request.Attributes, request.FarmIds, request.DataGrantId, (previous?.PolicyVersion ?? 0) + 1, previous?.Id, request.Locale, DateTimeOffset.UtcNow, actor.Subject); db.RetailDisclosurePolicies.Add(policy); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/retail-transparency/policies/{id}", policy); }
        catch (ArgumentException e) { return Bad("disclosure_policy_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}
