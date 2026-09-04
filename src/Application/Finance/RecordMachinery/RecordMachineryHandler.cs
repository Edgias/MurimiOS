using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.RecordMachinery;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using MachineryCostEntry = global::EdgIQ.MurimiOS.Domain.MachineryCostEntry.MachineryCostEntry;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RecordMachineryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<RecordMachineryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RecordMachineryCommand message, CancellationToken cancellationToken) => RecordMachineryAsync(message.farmId, message.request, db, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RecordMachineryAsync(Guid farmId, RecordMachineryCostRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await FarmAllowedAsync(farmId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        string? scopeError = await ValidateOperationalTargetAsync(farmId, request.CostCentreId, request.FieldOperationId,
            request.CropZoneId, request.LivestockSubjectId, request.Currency, db, cancellationToken);
        if (scopeError is not null) return Bad("machinery_scope_invalid", scopeError);
        decimal total = decimal.Round(request.DurationHours * request.HourlyRate + request.AdditionalCost, 2, MidpointRounding.AwayFromZero);
        if (total <= 0) return Bad("machinery_cost_invalid", "Machinery usage must produce a positive cost.");
        Guid id = NewId(request.Id); Guid transactionId = NewId(request.TransactionId); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var entry = new MachineryCostEntry(id, farmId, transactionId, request.CostCentreId, request.FieldOperationId,
                request.CropZoneId, request.LivestockSubjectId, request.MachineReference, request.OccurredAt,
                request.DurationHours, request.HourlyRate, request.AdditionalCost, request.Currency, request.Notes);
            var transaction = new FinanceTransaction(transactionId, farmId, FinanceEntryType.Expense, "Machinery",
                entry.TotalCost, entry.Currency, entry.OccurredAt, FinanceSourceType.Machinery, id, request.Reference,
                request.MachineReference, request.Notes, now, actor.Subject);
            var allocation = new CostAllocation(Guid.NewGuid(), transactionId, request.CostCentreId, entry.TotalCost,
                100m, CostAllocationMethod.Direct, "Direct machinery allocation");
            db.MachineryCostEntries.Add(entry); db.FinanceTransactions.Add(transaction); db.CostAllocations.Add(allocation);
            var response = new MachineryCostResponse(id, transactionId, farmId, request.CostCentreId,
                request.FieldOperationId, request.CropZoneId, request.LivestockSubjectId, entry.MachineReference,
                entry.OccurredAt, entry.EnteredDurationHours, entry.DurationMinutes, entry.HourlyRate,
                entry.AdditionalCost, entry.TotalCost, entry.Currency, entry.Notes);
            AddRecords(db, tenant, actor, "MachineryCostRecorded", "Finance.MachineryCostRecorded",
                nameof(MachineryCostEntry), id, farmId, response, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/finance/machinery-entries/{id}", response);
        }
        catch (ArgumentException ex) { return Bad("machinery_cost_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("machinery_cost_conflict", ex.InnerException?.Message ?? "The machinery record or reference already exists."); }
    }


    private static async Task<bool> FarmAllowedAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken) =>
        await scope.CanAccessFarmAsync(farmId, cancellationToken)
        && await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken);


    private static async Task<string?> ValidateOperationalTargetAsync(Guid farmId, Guid costCentreId,
        Guid? operationId, Guid? cropZoneId, Guid? livestockSubjectId, string currency,
        IPlatformDbContext db, CancellationToken cancellationToken)
    {
        CostCentre? centre = await db.CostCentres.AsNoTracking().SingleOrDefaultAsync(x => x.Id == costCentreId, cancellationToken);
        if (centre is null || centre.FarmId != farmId || !centre.IsActive) return "The cost centre is unavailable for this farm.";
        if (!centre.Currency.Equals(currency?.Trim(), StringComparison.OrdinalIgnoreCase)) return "Cost currency must match the cost centre currency.";
        Guid? operationFieldId = null;
        if (operationId is { } opId)
        {
            operationFieldId = await (from operation in db.FieldOperations.AsNoTracking()
                join field in db.FarmFields.AsNoTracking() on operation.FieldId equals field.Id
                where operation.Id == opId && field.FarmId == farmId select (Guid?)field.Id).SingleOrDefaultAsync(cancellationToken);
            if (operationFieldId is null) return "The field operation does not belong to this farm.";
        }
        if (cropZoneId is { } zoneId && !await (from zone in db.CropZones.AsNoTracking()
            join season in db.GrowingSeasons.AsNoTracking() on zone.SeasonId equals season.Id
            where zone.Id == zoneId && season.FarmId == farmId select zone.Id).AnyAsync(cancellationToken))
            return "The crop zone does not belong to this farm.";
        if (livestockSubjectId is { } subjectId && !await db.LivestockSubjects.AsNoTracking()
            .AnyAsync(x => x.Id == subjectId && x.FarmId == farmId, cancellationToken))
            return "The livestock subject does not belong to this farm.";
        if (centre.Type == CostCentreType.CropZone && centre.TargetId != cropZoneId)
            return "The crop-zone cost centre does not match the operational target.";
        if (centre.Type == CostCentreType.LivestockSubject && centre.TargetId != livestockSubjectId)
            return "The livestock cost centre does not match the operational target.";
        if (centre.Type == CostCentreType.Field && centre.TargetId != operationFieldId)
            return "The field cost centre requires a matching field operation.";
        return null;
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid farmId, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(payload),
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        string syncOperation = action.Contains("Created", StringComparison.Ordinal)
            || action.Contains("Posted", StringComparison.Ordinal)
            || action.Contains("Recorded", StringComparison.Ordinal)
            || action.Contains("Imported", StringComparison.Ordinal) ? "Created" : "Updated";
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entityType, entityId, syncOperation,
            JsonSerializer.Serialize(payload), now));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType,
            JsonSerializer.Serialize(payload), now));
    }


    private static Guid NewId(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
