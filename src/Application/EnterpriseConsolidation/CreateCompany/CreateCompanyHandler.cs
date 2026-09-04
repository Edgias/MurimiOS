using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateCompany;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateCompanyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateCompanyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCompanyCommand message, CancellationToken cancellationToken) => CreateCompanyAsync(message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateCompanyAsync(CreateLegalCompanyRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock,
        UseCaseContext context, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        if (!resolved.AllowsNode(request.OrganisationNodeId)) return UseCaseResults.NotFound();
        OrganisationNode? node = await db.OrganisationNodes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.OrganisationNodeId, ct);
        if (node is null) return UseCaseResults.NotFound();
        if (node.NodeType != OrganisationNodeType.Organisation)
            return Bad("legal_company_node_invalid", "A legal company must be registered against an organisation node.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            var company = new LegalCompany(Id(request.Id), request.OrganisationNodeId, request.CompanyCode,
                request.LegalName, request.RegistrationNumber, request.TaxIdentifier, request.CountryCode,
                request.FunctionalCurrency, request.EffectiveFrom, actor.Subject);
            db.LegalCompanies.Add(company); AddRecords(db, tenant, actor, "LegalCompanyRegistered", "Enterprise.LegalCompanyRegistered",
                nameof(LegalCompany), company.Id, company.OrganisationNodeId, company, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/enterprise/legal-companies/{company.Id}", company);
        }
        catch (ArgumentException ex) { return Bad("legal_company_invalid", ex.Message); }
        catch (DbUpdateException) { return Conflict("legal_company_conflict", "The organisation, company code, or jurisdiction registration is already registered."); }
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
