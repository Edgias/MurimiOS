using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.ListRiskAssessments;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;

public sealed class ListRiskAssessmentsHandler(IPlatformDbContext db) : IRequestHandler<ListRiskAssessmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRiskAssessmentsQuery message, CancellationToken cancellationToken) => ListRiskAssessmentsAsync(message.relationshipId, message.hostedSupplierId, message.limit, db, cancellationToken);

    private static async Task<UseCaseResult> ListRiskAssessmentsAsync(Guid? relationshipId, Guid? hostedSupplierId,
        int? limit, IPlatformDbContext db, CancellationToken ct)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue)
            return Bad("supplier_target_invalid", "Choose one supplier target.");
        IQueryable<SupplierRiskAssessment> query = db.SupplierRiskAssessments.AsNoTracking();
        if (relationshipId.HasValue) query = query.Where(x => x.RelationshipId == relationshipId);
        else query = query.Where(x => x.HostedSupplierId == hostedSupplierId);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.AssessedAt).Take(Math.Clamp(limit ?? 20, 1, 100)).ToArrayAsync(ct));
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
