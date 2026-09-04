using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.Apply;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using OpeningBalanceDirection = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.OpeningBalanceDirection;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using CostAllocationMethod = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocationMethod;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
using MasterDataImportRowStatus = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRowStatus;
using MasterDataImportStatus = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportStatus;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ApplyHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, ILoggerFactory loggerFactory) : IRequestHandler<ApplyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ApplyCommand message, CancellationToken cancellationToken) => ApplyAsync(message.jobId, message.batchSize, db, tenant, actor, clock, loggerFactory, cancellationToken);

    private static async Task<UseCaseResult> ApplyAsync(Guid jobId, int? batchSize, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, ILoggerFactory loggerFactory,
        CancellationToken ct)
    {
        MasterDataImportJob? job = await db.MasterDataImportJobs.SingleOrDefaultAsync(x => x.Id == jobId, ct);
        if (job is null) return UseCaseResults.NotFound();
        if (job.Status == MasterDataImportStatus.Applied)
            return UseCaseResults.Ok(new { job = Response(job), batchApplied = 0, completed = true });
        if (job.Status is not (MasterDataImportStatus.Validated or MasterDataImportStatus.Applying))
            return Conflict("master_data_import_not_ready", "Only a validated import can be applied.");
        int take = Math.Clamp(batchSize ?? 250, 1, 1_000);
        DateTimeOffset now = clock.GetUtcNow();
        MasterDataImportRow[] rows = await db.MasterDataImportRows
            .Where(x => x.JobId == jobId && x.Status == MasterDataImportRowStatus.Ready && x.Ordinal > job.CheckpointOrdinal)
            .OrderBy(x => x.Ordinal).Take(take).ToArrayAsync(ct);
        job.BeginApply(actor.Subject, now);
        if (rows.Length == 0)
        {
            job.RecordBatch(job.CheckpointOrdinal, job.AppliedRowCount, job.FailedRowCount, true, now);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Ok(Response(job));
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        try
        {
            foreach (MasterDataImportRow row in rows)
            {
                Dictionary<string, string> values = JsonSerializer.Deserialize<Dictionary<string, string>>(row.NormalizedJson, JsonOptions)!;
                AppliedEntity applied = await ApplyRowAsync(job.Type, row, values, db, tenant, actor, now, ct);
                row.MarkApplied(applied.EntityType, applied.EntityId, applied.FarmId, now);
                string payload = JsonSerializer.Serialize(new { ImportJobId = job.Id, ImportRowId = row.Id,
                    job.Type, row.Ordinal, applied.EntityType, applied.EntityId, applied.FarmId, Values = values }, JsonOptions);
                AddAudit(db, tenant, actor, now, "Imported", applied.EntityType, applied.EntityId, applied.FarmId,
                    new { ImportJobId = job.Id, ImportRowId = row.Id, row.Ordinal, job.Type, values });
                db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId,
                    $"MasterData.{job.Type}.Imported", payload, now));
                if (applied.FarmId is { } farmId)
                    db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, applied.EntityType,
                        applied.EntityId, "MasterDataImported", payload, now));
            }
            int checkpoint = rows[^1].Ordinal;
            bool completed = !await db.MasterDataImportRows.AnyAsync(x => x.JobId == jobId
                && x.Status == MasterDataImportRowStatus.Ready && x.Ordinal > checkpoint, ct);
            int newAppliedCount = job.AppliedRowCount + rows.Length;
            job.RecordBatch(checkpoint, newAppliedCount, job.FailedRowCount, completed, now);
            if (completed)
            {
                AddAudit(db, tenant, actor, now, "Applied", nameof(MasterDataImportJob), job.Id, null,
                    new { job.Id, job.Type, AppliedRows = newAppliedCount, job.ErrorRowCount });
                db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "MasterDataImport.Applied",
                    JsonSerializer.Serialize(new { job.Id, job.Type, AppliedRows = newAppliedCount }), now));
            }
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return UseCaseResults.Ok(new { job = Response(job), batchApplied = rows.Length, completed });
        }
        catch (Exception exception) when (exception is DbUpdateException or DbException or ArgumentException or InvalidOperationException)
        {
            await transaction.RollbackAsync(ct);
            loggerFactory.CreateLogger("MasterDataImport").LogWarning(exception,
                "Master-data import batch {ImportJobId} failed after checkpoint {CheckpointOrdinal}.",
                job.Id, job.CheckpointOrdinal);
            db.ChangeTracker.Clear();
            return Conflict("master_data_apply_conflict", "The current batch conflicts with data changed since validation. No rows in the batch were committed.");
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static async Task<AppliedEntity> ApplyRowAsync(MasterDataImportType type, MasterDataImportRow row,
        Dictionary<string, string> v, IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        DateTimeOffset now, CancellationToken ct)
    {
        switch (type)
        {
            case MasterDataImportType.Farms:
            {
                Guid nodeId = Derived(row.Id, 1); Guid farmId = row.Id;
                Guid? parentId = Guid.TryParse(v.GetValueOrDefault("parent_organisation_node_id"), out Guid parent) ? parent : null;
                string? parentPath = parentId.HasValue ? await db.OrganisationNodes.Where(x => x.Id == parentId).Select(x => x.Path).SingleAsync(ct) : null;
                var node = new OrganisationNode(nodeId, v["name"], OrganisationNodeType.Farm,
                    parentPath is null ? $"farm_{nodeId:N}" : $"{parentPath}.farm_{nodeId:N}", parentId);
                var farm = new Farm(farmId, nodeId, v["name"], v["code"], v["country_code"], v["time_zone"]);
                db.OrganisationNodes.Add(node); db.Farms.Add(farm);
                return new(nameof(Farm), farmId, farmId);
            }
            case MasterDataImportType.Fields:
            {
                Farm farm = await db.Farms.SingleAsync(x => x.Code == v["farm_code"].ToUpper(), ct);
                var field = new FarmField(row.Id, farm.Id, v["name"]);
                db.FarmFields.Add(field);
                db.FieldBoundaryVersions.Add(new FieldBoundaryVersion(Derived(row.Id, 1), field.Id, Boundary(v["boundary_wkt"]),
                    DateOnly.Parse(v["valid_from"], CultureInfo.InvariantCulture), EmptyDefault(v, "source", "csv-import")));
                return new(nameof(FarmField), field.Id, farm.Id);
            }
            case MasterDataImportType.Livestock:
            {
                Farm farm = await db.Farms.SingleAsync(x => x.Code == v["farm_code"].ToUpper(), ct);
                var subject = new LivestockSubject(row.Id, farm.Id, Enum.Parse<LivestockSubjectKind>(v["kind"], true),
                    v["name"], v["species"], Null(v, "official_identifier"));
                db.LivestockSubjects.Add(subject); return new(nameof(LivestockSubject), subject.Id, farm.Id);
            }
            case MasterDataImportType.Inventory:
            {
                Farm farm = await db.Farms.SingleAsync(x => x.Code == v["farm_code"].ToUpper(), ct);
                AgriculturalInput? input = db.AgriculturalInputs.Local.FirstOrDefault(x => x.FarmId == farm.Id
                    && x.Name.ToUpper() == v["input_name"].ToUpper())
                    ?? await db.AgriculturalInputs.FirstOrDefaultAsync(x => x.FarmId == farm.Id && x.Name.ToUpper() == v["input_name"].ToUpper(), ct);
                if (input is null)
                {
                    input = new AgriculturalInput(Derived(row.Id, 1), farm.Id, v["input_name"], v["input_category"], v["unit"]);
                    db.AgriculturalInputs.Add(input);
                }
                var lot = new InputStockLot(row.Id, input.Id, v["lot_code"], DateOnly.Parse(v["received_on"], CultureInfo.InvariantCulture),
                    Date(v, "expires_on"), decimal.Parse(v["quantity"], NumberStyles.Number, CultureInfo.InvariantCulture), v["unit"]);
                db.InputStockLots.Add(lot); return new(nameof(InputStockLot), lot.Id, farm.Id);
            }
            case MasterDataImportType.Suppliers:
            {
                var supplier = new HostedSupplierAccount(row.Id, v["supplier_code"], v["name"], v["contact_name"], v["email"], Null(v, "phone"));
                db.HostedSupplierAccounts.Add(supplier); return new(nameof(HostedSupplierAccount), supplier.Id, null);
            }
            case MasterDataImportType.OpeningBalances:
            {
                Farm farm = await db.Farms.SingleAsync(x => x.Code == v["farm_code"].ToUpper(), ct);
                var balance = new FinanceOpeningBalance(row.Id, farm.Id, v["account_code"], v["account_name"],
                    Enum.Parse<OpeningBalanceDirection>(v["direction"], true),
                    decimal.Parse(v["amount"], NumberStyles.Number, CultureInfo.InvariantCulture), v["currency"],
                    DateOnly.Parse(v["as_of"], CultureInfo.InvariantCulture), v["reference"], now, actor.Subject);
                db.FinanceOpeningBalances.Add(balance); return new(nameof(FinanceOpeningBalance), balance.Id, farm.Id);
            }
            case MasterDataImportType.FinanceTransactions:
            {
                Farm farm = await db.Farms.SingleAsync(x => x.Code == v["farm_code"].ToUpper(), ct);
                var finance = new FinanceTransaction(row.Id, farm.Id, Enum.Parse<FinanceEntryType>(v["entry_type"], true),
                    v["category"], decimal.Parse(v["amount"], NumberStyles.Number, CultureInfo.InvariantCulture), v["currency"],
                    DateTimeOffset.Parse(v["occurred_at"], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
                    FinanceSourceType.Accounting, row.Id, v["reference"], Null(v, "counterparty"), Null(v, "description"), now, actor.Subject);
                string centreCode = $"CSV-IMPORT-{finance.Currency}";
                CostCentre? centre = db.CostCentres.Local.FirstOrDefault(x => x.FarmId == farm.Id && x.Code == centreCode)
                    ?? await db.CostCentres.SingleOrDefaultAsync(x => x.FarmId == farm.Id && x.Code == centreCode, ct);
                if (centre is null)
                {
                    centre = new CostCentre(Derived(farm.Id, centreCode), farm.Id, centreCode,
                        $"CSV import clearing ({finance.Currency})", CostCentreType.Farm, farm.Id, finance.Currency);
                    db.CostCentres.Add(centre);
                    object centrePayload = new { centre.Id, centre.FarmId, centre.Code, centre.Name,
                        centre.Type, centre.TargetId, centre.Currency, ImportRowId = row.Id };
                    AddAudit(db, tenant, actor, now, "CostCentreCreated", nameof(CostCentre), centre.Id, farm.Id, centrePayload);
                    string centreJson = JsonSerializer.Serialize(centrePayload, JsonOptions);
                    db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId,
                        "Finance.CostCentreCreated", centreJson, now));
                    db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm.Id, nameof(CostCentre), centre.Id,
                        "Created", centreJson, now));
                }
                else if (!centre.IsActive || centre.Type != CostCentreType.Farm || centre.TargetId != farm.Id
                    || !centre.Currency.Equals(finance.Currency, StringComparison.Ordinal))
                    throw new InvalidOperationException($"Cost centre {centreCode} is not an active farm-level {finance.Currency} import clearing centre.");

                var allocation = new CostAllocation(Derived(row.Id, 2), finance.Id, centre.Id, finance.Amount,
                    100m, CostAllocationMethod.Direct, "Created by the bulk master-data importer.");
                db.FinanceTransactions.Add(finance);
                db.CostAllocations.Add(allocation);
                return new(nameof(FinanceTransaction), finance.Id, farm.Id);
            }
            default: throw new UnreachableException();
        }
    }

    private static MultiPolygon Boundary(string wkt)
    {
        Geometry geometry = new WKTReader(NtsGeometryServices.Instance).Read(wkt);
        MultiPolygon result = geometry switch
        {
            Polygon polygon => geometry.Factory.CreateMultiPolygon([polygon]),
            MultiPolygon multi => multi,
            _ => throw new ArgumentException("boundary_wkt must contain a Polygon or MultiPolygon.")
        };
        result.SRID = 4326;
        if (result.IsEmpty || !result.IsValid || result.Coordinates.Any(x => x.X is < -180 or > 180 || x.Y is < -90 or > 90))
            throw new ArgumentException("boundary_wkt must be a valid WGS84 polygon.");
        return result;
    }

    private static DateOnly? Date(Dictionary<string, string> values, string key) =>
        values.GetValueOrDefault(key) is { Length: > 0 } value ? DateOnly.Parse(value, CultureInfo.InvariantCulture) : null;

    private static string? Null(Dictionary<string, string> values, string key) =>
        values.GetValueOrDefault(key) is { Length: > 0 } value ? value : null;

    private static string EmptyDefault(Dictionary<string, string> values, string key, string fallback) => Null(values, key) ?? fallback;

    private static Guid Derived(Guid source, byte discriminator)
    {
        byte[] input = new byte[17]; source.TryWriteBytes(input); input[16] = discriminator;
        return new Guid(SHA256.HashData(input).AsSpan(0, 16));
    }

    private static Guid Derived(Guid source, string discriminator) => new(
        SHA256.HashData(Encoding.UTF8.GetBytes($"{source:N}:{discriminator}")).AsSpan(0, 16));

    private static object Response(MasterDataImportJob x) => new
    {
        x.Id, x.Type, x.FileName, x.ByteSize, x.Sha256, x.ParserVersion, x.Status, x.RowCount,
        x.ReadyRowCount, x.ErrorRowCount, x.AppliedRowCount, x.FailedRowCount, x.CheckpointOrdinal,
        x.ValidatedAt, x.ApplyStartedAt, x.AppliedAt, x.AppliedBy, x.FailureCode, x.FailureDetail, x.CreatedAtUtc
    };

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        DateTimeOffset at, string action, string entityType, Guid entityId, Guid? farmId, object changes) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, at, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId,
            JsonSerializer.Serialize(changes, JsonOptions), Activity.Current?.TraceId.ToString()));

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private sealed record AppliedEntity(string EntityType, Guid EntityId, Guid? FarmId);

}
