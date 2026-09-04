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
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Application.Common.Imports;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.MasterDataImport.CreateJob;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using OpeningBalanceDirection = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.OpeningBalanceDirection;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using FinanceSourceType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceSourceType;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, IMediaStorage storage, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock) : IRequestHandler<CreateJobCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateJobCommand message, CancellationToken cancellationToken) => CreateJobAsync(message.type, message.request, db, scopeAccess, storage, tenant, actor, clock, cancellationToken);

    private static async Task<UseCaseResult> CreateJobAsync(string type, IncomingContent request, IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess, IMediaStorage storage, ITenantContext tenant,
        IRequestActorContext actor, TimeProvider clock, CancellationToken ct)
    {
        if (!TryType(type, out MasterDataImportType target)) return Bad("master_data_type_invalid", TypeMessage());
        if (request.ContentLength is > MaximumBytes) return TooLarge();
        byte[] bytes;
        try { bytes = await ReadLimitedAsync(request.Body, MaximumBytes, ct); }
        catch (ImportTooLargeException) { return TooLarge(); }
        if (bytes.Length == 0) return Bad("csv_empty", "The request body is empty.");
        string sha256 = Convert.ToHexStringLower(SHA256.HashData(bytes));
        MasterDataImportJob? existing = await db.MasterDataImportJobs.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Type == target && x.Sha256 == sha256, ct);
        if (existing is not null) return UseCaseResults.Ok(Response(existing));

        MasterDataCsvParseResult parsed = MasterDataCsv.Parse(target, bytes);
        Guid jobId = Guid.NewGuid();
        string fileName = SanitizeFileName(request.FirstQuery("fileName")
            ?? request.FirstHeader("X-File-Name") ?? "");
        if (string.IsNullOrWhiteSpace(fileName)) fileName = $"{Slug(target)}.csv";
        await using var source = new MemoryStream(bytes, writable: false);
        string objectKey = await storage.PutImportAsync(tenant.TenantId, jobId, "text/csv", source, bytes.Length, ct);
        var job = new MasterDataImportJob(jobId, target, fileName, bytes.Length, sha256, objectKey,
            MasterDataCsv.Version, JsonSerializer.Serialize(parsed.Header, JsonOptions));
        db.MasterDataImportJobs.Add(job);
        DateTimeOffset now = clock.GetUtcNow();

        if (parsed.FileIssues.Count > 0)
        {
            MasterDataCsvIssue issue = parsed.FileIssues[0];
            job.Fail(issue.Code, issue.Message, now);
        }
        else
        {
            ValidationState state = await ValidationState.CreateAsync(target, db, scopeAccess, ct);
            int ready = 0;
            foreach (ParsedMasterDataRow sourceRow in parsed.Rows)
            {
                ValidatedRow validated = Validate(target, sourceRow, state);
                if (validated.Issues.Count == 0) ready++;
                db.MasterDataImportRows.Add(new MasterDataImportRow(Guid.NewGuid(), job.Id, sourceRow.Ordinal,
                    JsonSerializer.Serialize(sourceRow.Values, JsonOptions),
                    JsonSerializer.Serialize(validated.Values, JsonOptions),
                    JsonSerializer.Serialize(validated.Issues, JsonOptions), validated.Issues.Count == 0));
            }
            job.CompleteValidation(parsed.Rows.Count, ready, parsed.Rows.Count - ready, now);
        }

        AddAudit(db, tenant, actor, now, "Validated", nameof(MasterDataImportJob), job.Id, null,
            new { job.Type, job.FileName, job.Sha256, job.Status, job.RowCount, job.ReadyRowCount, job.ErrorRowCount });
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "MasterDataImport.Validated",
            JsonSerializer.Serialize(new { job.Id, job.Type, job.Status, job.RowCount, job.ReadyRowCount }), now));
        await db.SaveChangesAsync(ct);
        return UseCaseResults.Created($"/api/v1/imports/master-data/jobs/{job.Id}", Response(job));
    }

    private const long MaximumBytes = 20_971_520;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static ValidatedRow Validate(MasterDataImportType type, ParsedMasterDataRow row, ValidationState state)
    {
        var values = row.Values.ToDictionary(x => x.Key, x => x.Value.Trim(), StringComparer.Ordinal);
        var issues = new List<MasterDataCsvIssue>();
        foreach (string column in MasterDataCsv.TemplateColumns(type))
            if (!MasterDataCsv.IsOptional(type, column) && (!values.TryGetValue(column, out string? value) || value.Length == 0))
                issues.Add(new("required", column, $"{column} is required."));
        if (issues.Count > 0) return new(values, issues);
        try
        {
            switch (type)
            {
                case MasterDataImportType.Farms: ValidateFarm(values, state, issues); break;
                case MasterDataImportType.Fields: ValidateField(values, state, issues); break;
                case MasterDataImportType.Livestock: ValidateLivestock(values, state, issues); break;
                case MasterDataImportType.Inventory: ValidateInventory(values, state, issues); break;
                case MasterDataImportType.Suppliers: ValidateSupplier(values, state, issues); break;
                case MasterDataImportType.OpeningBalances: ValidateOpening(values, state, issues); break;
                case MasterDataImportType.FinanceTransactions: ValidateFinance(values, state, issues); break;
            }
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or FormatException)
        {
            issues.Add(new("row_invalid", null, exception.Message));
        }
        return new(values, issues);
    }


    private static void ValidateFarm(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        if (!state.Unrestricted) { issues.Add(new("tenant_scope_required", null, "Farm imports require unrestricted tenant scope.")); return; }
        string code = v["code"].ToUpperInvariant(); v["code"] = code;
        v["country_code"] = v["country_code"].ToUpperInvariant();
        if (!state.FarmCodes.Add(code)) issues.Add(new("farm_code_exists", "code", "Farm code already exists or is duplicated in this file."));
        Guid parentId = Guid.Empty;
        if (v.GetValueOrDefault("parent_organisation_node_id") is { Length: > 0 } parent
            && (!Guid.TryParse(parent, out parentId) || !state.OrganisationNodeIds.Contains(parentId)))
            issues.Add(new("parent_organisation_invalid", "parent_organisation_node_id", "Parent organisation node is unavailable."));
        _ = new Farm(Guid.NewGuid(), Guid.NewGuid(), v["name"], code, v["country_code"], v["time_zone"]);
    }


    private static void ValidateField(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        Farm? farm = FarmFor(v, state, issues); if (farm is null) return;
        string key = $"{farm.Id:N}|{v["name"].ToUpperInvariant()}";
        if (!state.FieldKeys.Add(key)) issues.Add(new("field_name_exists", "name", "Field name already exists or is duplicated for this farm."));
        MultiPolygon boundary = Boundary(v["boundary_wkt"]); v["boundary_wkt"] = new WKTWriter().Write(boundary);
        _ = DateOnly.Parse(v["valid_from"], CultureInfo.InvariantCulture);
        _ = new FarmField(Guid.NewGuid(), farm.Id, v["name"]);
        _ = new FieldBoundaryVersion(Guid.NewGuid(), Guid.NewGuid(), boundary,
            DateOnly.Parse(v["valid_from"], CultureInfo.InvariantCulture), EmptyDefault(v, "source", "csv-import"));
    }


    private static void ValidateLivestock(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        Farm? farm = FarmFor(v, state, issues); if (farm is null) return;
        if (!Enum.TryParse(v["kind"], true, out LivestockSubjectKind kind) || !Enum.IsDefined(kind))
            issues.Add(new("livestock_kind_invalid", "kind", "Use Individual or Group."));
        if (v.GetValueOrDefault("official_identifier") is { Length: > 0 } official && !state.LivestockIdentifiers.Add(official.ToUpperInvariant()))
            issues.Add(new("official_identifier_exists", "official_identifier", "Official identifier already exists or is duplicated."));
        if (issues.Count == 0) _ = new LivestockSubject(Guid.NewGuid(), farm.Id, kind, v["name"], v["species"], Null(v, "official_identifier"));
    }


    private static void ValidateInventory(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        Farm? farm = FarmFor(v, state, issues); if (farm is null) return;
        string key = $"{farm.Id:N}|{v["input_name"].ToUpperInvariant()}|{v["lot_code"].ToUpperInvariant()}";
        if (!state.InventoryKeys.Add(key)) issues.Add(new("inventory_lot_exists", "lot_code", "Input lot already exists or is duplicated."));
        DateOnly received = DateOnly.Parse(v["received_on"], CultureInfo.InvariantCulture);
        DateOnly? expires = Date(v, "expires_on");
        decimal quantity = decimal.Parse(v["quantity"], NumberStyles.Number, CultureInfo.InvariantCulture);
        var input = new AgriculturalInput(Guid.NewGuid(), farm.Id, v["input_name"], v["input_category"], v["unit"]);
        _ = new InputStockLot(Guid.NewGuid(), input.Id, v["lot_code"], received, expires, quantity, v["unit"]);
    }


    private static void ValidateSupplier(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        v["supplier_code"] = v["supplier_code"].ToUpperInvariant();
        if (!state.SupplierCodes.Add(v["supplier_code"])) issues.Add(new("supplier_code_exists", "supplier_code", "Supplier code already exists or is duplicated."));
        _ = new HostedSupplierAccount(Guid.NewGuid(), v["supplier_code"], v["name"], v["contact_name"], v["email"], Null(v, "phone"));
    }


    private static void ValidateOpening(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        Farm? farm = FarmFor(v, state, issues); if (farm is null) return;
        if (!Enum.TryParse(v["direction"], true, out OpeningBalanceDirection direction) || !Enum.IsDefined(direction))
            issues.Add(new("balance_direction_invalid", "direction", "Use Debit or Credit."));
        DateOnly asOf = DateOnly.Parse(v["as_of"], CultureInfo.InvariantCulture);
        string key = $"{farm.Id:N}|{v["account_code"].ToUpperInvariant()}|{asOf:O}";
        if (!state.OpeningKeys.Add(key)) issues.Add(new("opening_balance_exists", "account_code", "Opening balance already exists or is duplicated for this date."));
        if (issues.Count == 0) _ = new FinanceOpeningBalance(Guid.NewGuid(), farm.Id, v["account_code"], v["account_name"],
            direction, decimal.Parse(v["amount"], NumberStyles.Number, CultureInfo.InvariantCulture), v["currency"], asOf,
            v["reference"], DateTimeOffset.UtcNow, "validator");
    }


    private static void ValidateFinance(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        Farm? farm = FarmFor(v, state, issues); if (farm is null) return;
        if (!Enum.TryParse(v["entry_type"], true, out FinanceEntryType entryType) || !Enum.IsDefined(entryType))
            issues.Add(new("finance_entry_type_invalid", "entry_type", "Use Expense or Revenue."));
        string key = $"{farm.Id:N}|{v["reference"].ToUpperInvariant()}";
        if (!state.FinanceKeys.Add(key)) issues.Add(new("finance_reference_exists", "reference", "Finance reference already exists or is duplicated for this farm."));
        if (issues.Count == 0) _ = new FinanceTransaction(Guid.NewGuid(), farm.Id, entryType, v["category"],
            decimal.Parse(v["amount"], NumberStyles.Number, CultureInfo.InvariantCulture), v["currency"],
            DateTimeOffset.Parse(v["occurred_at"], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal),
            FinanceSourceType.Accounting, Guid.NewGuid(), v["reference"], Null(v, "counterparty"), Null(v, "description"),
            DateTimeOffset.UtcNow, "validator");
    }


    private static IEnumerable<IEnumerable<string?>> Header(MasterDataImportType type) => [MasterDataCsv.TemplateColumns(type)];

    private static Farm? FarmFor(Dictionary<string, string> v, ValidationState state, List<MasterDataCsvIssue> issues)
    {
        string code = v["farm_code"].ToUpperInvariant(); v["farm_code"] = code;
        if (!state.Farms.TryGetValue(code, out Farm? farm)) issues.Add(new("farm_not_found", "farm_code", "Farm is unavailable in the current organisation scope."));
        return farm;
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

    private static bool TryType(string value, out MasterDataImportType type) =>
        Enum.TryParse(value.Replace("-", ""), true, out type) && Enum.IsDefined(type);

    private static string TypeMessage() => "Use Farms, Fields, Livestock, Inventory, Suppliers, OpeningBalances, or FinanceTransactions.";

    private static string Slug(MasterDataImportType type) => type.ToString().ToLowerInvariant();

    private static string SanitizeFileName(string value)
    {
        string fileName = string.Concat(Path.GetFileName(value).Where(x => !char.IsControl(x))).Trim();
        return fileName[..Math.Min(255, fileName.Length)];
    }

    private static async Task<byte[]> ReadLimitedAsync(Stream input, long maximum, CancellationToken ct)
    {
        using var output = new MemoryStream(); byte[] buffer = new byte[64 * 1024];
        while (true) { int read = await input.ReadAsync(buffer, ct); if (read == 0) break;
            if (output.Length + read > maximum) throw new ImportTooLargeException(); await output.WriteAsync(buffer.AsMemory(0, read), ct); }
        return output.ToArray();
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult TooLarge() => UseCaseResults.Problem(statusCode: 413, title: "CSV import file too large",
        extensions: new Dictionary<string, object?> { ["code"] = "csv_too_large", ["maximumBytes"] = MaximumBytes });

    private sealed class ImportTooLargeException : Exception;

    private sealed record ValidatedRow(Dictionary<string, string> Values, List<MasterDataCsvIssue> Issues);


    private sealed class ValidationState
    {
        public required bool Unrestricted { get; init; }
        public required Dictionary<string, Farm> Farms { get; init; }
        public required HashSet<string> FarmCodes { get; init; }
        public required HashSet<Guid> OrganisationNodeIds { get; init; }
        public required HashSet<string> FieldKeys { get; init; }
        public required HashSet<string> LivestockIdentifiers { get; init; }
        public required HashSet<string> InventoryKeys { get; init; }
        public required HashSet<string> SupplierCodes { get; init; }
        public required HashSet<string> OpeningKeys { get; init; }
        public required HashSet<string> FinanceKeys { get; init; }

        public static async Task<ValidationState> CreateAsync(MasterDataImportType type, IPlatformDbContext db,
            IOrganisationScopeAccess access, CancellationToken ct)
        {
            ResolvedOrganisationScope scope = await access.ResolveAsync(ct);
            IQueryable<Farm> farmQuery = db.Farms.AsNoTracking();
            if (!scope.IsUnrestricted) farmQuery = farmQuery.Where(x => scope.FarmIds!.Contains(x.Id));
            Farm[] farms = await farmQuery.ToArrayAsync(ct); Guid[] farmIds = farms.Select(x => x.Id).ToArray();
            var codes = farms.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);
            var fields = (await db.FarmFields.AsNoTracking().Where(x => farmIds.Contains(x.FarmId))
                .Select(x => new { x.FarmId, x.Name }).ToArrayAsync(ct))
                .Select(x => $"{x.FarmId:N}|{x.Name.ToUpperInvariant()}").ToHashSet(StringComparer.Ordinal);
            Guid[] inputIds = await db.AgriculturalInputs.AsNoTracking().Where(x => farmIds.Contains(x.FarmId)).Select(x => x.Id).ToArrayAsync(ct);
            var inputs = await db.AgriculturalInputs.AsNoTracking().Where(x => inputIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct);
            var inventory = (await db.InputStockLots.AsNoTracking().Where(x => inputIds.Contains(x.InputId))
                .Select(x => new { x.InputId, x.LotCode }).ToArrayAsync(ct))
                .Select(x => $"{inputs[x.InputId].FarmId:N}|{inputs[x.InputId].Name.ToUpperInvariant()}|{x.LotCode.ToUpperInvariant()}").ToHashSet();
            var opening = (await db.FinanceOpeningBalances.AsNoTracking().Where(x => farmIds.Contains(x.FarmId))
                .Select(x => new { x.FarmId, x.AccountCode, x.AsOf }).ToArrayAsync(ct))
                .Select(x => $"{x.FarmId:N}|{x.AccountCode.ToUpperInvariant()}|{x.AsOf:O}").ToHashSet();
            var finance = (await db.FinanceTransactions.AsNoTracking().Where(x => farmIds.Contains(x.FarmId))
                .Select(x => new { x.FarmId, x.Reference }).ToArrayAsync(ct))
                .Select(x => $"{x.FarmId:N}|{x.Reference.ToUpperInvariant()}").ToHashSet();
            return new ValidationState
            {
                Unrestricted = scope.IsUnrestricted,
                Farms = codes,
                FarmCodes = (await db.Farms.AsNoTracking().Select(x => x.Code).ToArrayAsync(ct)).ToHashSet(StringComparer.OrdinalIgnoreCase),
                OrganisationNodeIds = (await db.OrganisationNodes.AsNoTracking().Select(x => x.Id).ToArrayAsync(ct)).ToHashSet(),
                FieldKeys = fields,
                LivestockIdentifiers = (await db.LivestockSubjects.AsNoTracking().Where(x => x.OfficialIdentifier != null)
                    .Select(x => x.OfficialIdentifier!).ToArrayAsync(ct)).ToHashSet(StringComparer.OrdinalIgnoreCase),
                InventoryKeys = inventory,
                SupplierCodes = (await db.HostedSupplierAccounts.AsNoTracking().Select(x => x.SupplierCode).ToArrayAsync(ct)).ToHashSet(StringComparer.OrdinalIgnoreCase),
                OpeningKeys = opening,
                FinanceKeys = finance
            };
        }
    }

}
