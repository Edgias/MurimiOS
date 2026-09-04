using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.ListRelationships;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class ListRelationshipsHandler(IPlatformDbContext db, ITenantContext tenant) : IRequestHandler<ListRelationshipsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRelationshipsQuery message, CancellationToken cancellationToken) => ListRelationshipsAsync(db, tenant, cancellationToken);

    private static async Task<UseCaseResult> ListRelationshipsAsync(IPlatformDbContext db, ITenantContext tenant,
        CancellationToken cancellationToken)
    {
        SupplyRelationship[] rows = await db.SupplyRelationships.AsNoTracking()
            .Where(x => x.ProducerTenantId == tenant.TenantId || x.BuyerTenantId == tenant.TenantId)
            .OrderByDescending(x => x.RequestedAt).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToResponse));
    }

    private static SupplyRelationshipResponse ToResponse(SupplyRelationship x) => new(x.Id, x.ProducerTenantId,
        x.BuyerTenantId, x.RequestedByTenantId, x.RelationshipName, x.Status, x.RequestedAt, x.RespondedAt,
        x.RevokedAt, x.RevokedByTenantId, x.RevocationReason, x.Version);

    private static DataGrantResponse ToResponse(DataGrant x) => new(x.Id, x.GrantSeriesId, x.RelationshipId,
        x.ProducerTenantId, x.RecipientTenantId, x.Name, x.Purpose, x.EntityCategories, x.Attributes, x.FarmIds,
        x.SeasonIds, x.Commodities, x.ValidFrom, x.ValidTo, x.DownstreamDisclosureAllowed, x.GrantVersion,
        x.PreviousGrantId, x.Status, x.RevokedAt, x.RevocationReason);

}
