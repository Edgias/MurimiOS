using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CloseCompany;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CloseCompanyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CloseCompanyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CloseCompanyCommand message, CancellationToken cancellationToken) => CloseCompanyAsync(message.id, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CloseCompanyAsync(Guid id, CloseLegalCompanyRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        LegalCompany? company = await db.LegalCompanies.SingleOrDefaultAsync(x => x.Id == id, ct);
        if (company is null || !(await scope.ResolveAsync(ct)).AllowsNode(company.OrganisationNodeId)) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); company.Close(request.EffectiveTo, now, actor.Subject);
            AddRecords(db, tenant, actor, "LegalCompanyClosed", "Enterprise.LegalCompanyClosed", nameof(LegalCompany),
                company.Id, company.OrganisationNodeId, new { company.Id, company.EffectiveTo, company.ClosedAt }, now, context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(company);
        }
        catch (InvalidOperationException ex) { return Conflict("legal_company_transition_invalid", ex.Message); }
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid organisationNodeId,
        object payload, DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, null, organisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
