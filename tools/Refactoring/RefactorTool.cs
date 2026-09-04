using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// One-shot structural transformations. All generated edits preserve declaration
// bodies. The baseline archives are local recovery artifacts, not source control.
internal static partial class RefactorTool
{
    static string Root = Directory.GetCurrentDirectory();
    static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
    static string Full(string path)
    {
        string result = Path.GetFullPath(Path.Combine(Root, path));
        if (!result.StartsWith(Root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Outside workspace: {path}");
        return result;
    }
    static string Relative(string path) => Path.GetRelativePath(Root, path).Replace('\\', '/');
    static IEnumerable<string> Sources() => new[] { "src", "API", "Infrastructure", "Worker", "tests", "tools/SeedData" }
        .Where(p => Directory.Exists(Full(p))).SelectMany(p => Directory.EnumerateFiles(Full(p), "*.cs", SearchOption.AllDirectories))
        .Where(p => !Regex.IsMatch(Relative(p), @"/(bin|obj|Migrations)/", RegexOptions.IgnoreCase));
    static CompilationUnitSyntax Parse(string text) => CSharpSyntaxTree.ParseText(text, new CSharpParseOptions(LanguageVersion.Preview)).GetCompilationUnitRoot();
    static string Namespace(MemberDeclarationSyntax declaration) => declaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? "";
    static string TypeName(MemberDeclarationSyntax member) => member switch { BaseTypeDeclarationSyntax t => t.Identifier.ValueText, DelegateDeclarationSyntax d => d.Identifier.ValueText, _ => "" };
    static IEnumerable<MemberDeclarationSyntax> TopTypes(CompilationUnitSyntax root) => root.DescendantNodes().OfType<MemberDeclarationSyntax>()
        .Where(n => n is BaseTypeDeclarationSyntax or DelegateDeclarationSyntax && n.Parent is BaseNamespaceDeclarationSyntax or CompilationUnitSyntax);
    static string NewNamespace(string destination)
    {
        string path=destination.StartsWith("src/")?destination[4..]:destination.StartsWith("tests/")?destination[6..]:destination;
        return "EdgIQ.MurimiOS."+path[..path.LastIndexOf('/')].Replace('/', '.');
    }

    static async Task<int> Main(string[] args)
    {
        Root = Path.GetFullPath(args.Length > 1 ? args[1] : Directory.GetCurrentDirectory()).TrimEnd(Path.DirectorySeparatorChar);
        switch (args.FirstOrDefault())
        {
            case "domain": FinishDomain(); break;
            case "inventory": Inventory(); break;
            case "dedup": DeduplicateImports(); break;
            case "fix-common-imports": FixCommonImports(); break;
            case "dependencies": Dependencies(); break;
            case "verify-domain": VerifyDomain(); break;
            case "application-ports": ApplicationPorts(); break;
            case "sync-status": RelocateDeclarations([new("Infrastructure/Platform/Sync/SyncCommandLog.cs", "src/Domain/SyncClient/SyncCommandStatus.cs", "SyncCommandStatus")]); break;
            case "benchmark-port": RelocateDeclarations([new("Infrastructure/Platform/Analytics/BenchmarkAggregateProjection.cs", "src/Application/Common/Abstractions/BenchmarkAggregateProjection.cs", "BenchmarkAggregateProjection")]); break;
            case "application-services": ApplicationServices(); break;
            case "slices": ExtractSlices(args.Skip(2).ToArray()); break;
            case "repair-slices": RepairSlices(); break;
            case "shared-queries": ExtractSharedQueries(); break;
            case "application-aliases": RefreshApplicationAliases(); break;
            case "transport-repairs": RepairTransport(); break;
            case "finish-sync": FinishSync(); break;
            case "bin-metadata": RestoreBinMetadata(); break;
            case "infrastructure": RegroupInfrastructure(); break;
            case "infrastructure-resume": RegroupInfrastructure(true); break;
            case "split-registration": SplitApiRegistration(); break;
            case "infrastructure-aliases": RepairInfrastructureAliases(); break;
            case "api": RegroupApi(); break;
            case "api-resume": RegroupApi(true); break;
            case "api-aliases": RepairApiAliases(); break;
            case "finish-bin": FinishBinPlacement(); break;
            case "tests": RegroupTests(); break;
            case "prune-imports": PruneImports(); break;
            default: Console.WriteLine("Commands: domain, inventory [workspace]"); return 1;
        }
        await Task.CompletedTask;
        return 0;
    }

    static void Backup(string phase)
    {
        string path = Full($"output/refactoring/{phase}");
        if (Directory.Exists(path)) throw new InvalidOperationException($"Baseline already exists: {path}");
        Directory.CreateDirectory(path);
        var files = Sources().Concat(Directory.EnumerateFiles(Full("Infrastructure"), "*.cs", SearchOption.AllDirectories).Where(p => Relative(p).Contains("/Migrations/")))
            .Concat(Directory.EnumerateFiles(Root, "*.sln")).Concat(Directory.EnumerateFiles(Root, "*.props"))
            .Concat(new[] { "src", "API", "Infrastructure", "Worker", "tests", "tools/SeedData" }.Where(p => Directory.Exists(Full(p)))
                .SelectMany(p => Directory.EnumerateFiles(Full(p), "*.csproj", SearchOption.AllDirectories))).Distinct().ToArray();
        using var zip = ZipFile.Open(Path.Combine(path, "sources.zip"), ZipArchiveMode.Create);
        foreach (string file in files) zip.CreateEntryFromFile(file, Relative(file));
        File.WriteAllText(Path.Combine(path, "hashes.json"), JsonSerializer.Serialize(files.ToDictionary(Relative, f => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(f)))), Json));
    }

    static void Move(string source, string target)
    {
        string from = Full(source), to = Full(target);
        if (File.Exists(to)) throw new InvalidOperationException($"Target exists: {target}");
        Directory.CreateDirectory(Path.GetDirectoryName(to)!);
        // Untracked source moves do not stage existing user implementation.
        var check = Process.Start(new ProcessStartInfo("git") { ArgumentList = { "ls-files", "--error-unmatch", "--", source }, RedirectStandardError = true, RedirectStandardOutput = true, CreateNoWindow = true })!;
        check.WaitForExit();
        if (check.ExitCode == 0)
        {
            var move = Process.Start(new ProcessStartInfo("git") { ArgumentList = { "mv", "--", source, target }, CreateNoWindow = true })!;
            move.WaitForExit();
            if (move.ExitCode != 0) throw new InvalidOperationException($"git mv failed for {source}");
        }
        else File.Move(from, to);
    }

    static void PruneImports()
    {
        var roots=Sources().ToDictionary(f=>f,f=>Parse(File.ReadAllText(f)));
        var names=roots.Values.SelectMany(TopTypes).GroupBy(Namespace).ToDictionary(g=>g.Key,g=>g.Select(TypeName)
            .Concat(g.SelectMany(t=>t.DescendantNodes().OfType<MethodDeclarationSyntax>()).Where(m=>m.ParameterList.Parameters.FirstOrDefault()?.Modifiers.Any(SyntaxKind.ThisKeyword)==true).Select(m=>m.Identifier.ValueText)).ToHashSet());
        int count=0;
        foreach(var pair in roots.Where(p=>Relative(p.Key).StartsWith("src/Application/")||Relative(p.Key).StartsWith("src/Infrastructure/")||Relative(p.Key).StartsWith("src/API/")))
        {
            var root=pair.Value;
            var tokens=TopTypes(root).SelectMany(t=>t.DescendantTokens()).Where(t=>t.IsKind(SyntaxKind.IdentifierToken)).Select(t=>t.ValueText)
                .Concat(root.Members.OfType<GlobalStatementSyntax>().SelectMany(t=>t.DescendantTokens()).Where(t=>t.IsKind(SyntaxKind.IdentifierToken)).Select(t=>t.ValueText)).ToHashSet();
            var unused=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias==null&&!u.StaticKeyword.IsKind(SyntaxKind.StaticKeyword)&&!u.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                .Where(u=>names.TryGetValue(u.Name!.ToString(),out var symbols)&&!symbols.Overlaps(tokens)).ToArray();
            if(unused.Length>0){File.WriteAllText(pair.Key,root.RemoveNodes(unused,SyntaxRemoveOptions.KeepNoTrivia)!.ToFullString());count+=unused.Length;}
        }
        Console.WriteLine($"Removed {count} unused project-namespace imports; framework and external extension imports retained.");
    }

    static readonly Dictionary<string, string> Owners = new(StringComparer.Ordinal);
    static HashSet<string>? GenericTypeNames;
    static void Group(string owner, params string[] names) { foreach (string name in names) Owners[name] = owner; }
    static void DomainOwners()
    {
        Group("LandTenureRecord", "LandTenureType", "LandTenureStatus", "LandTenureRecord");
        Group("SiteInfrastructureAsset", "InfrastructureAssetType", "InfrastructureAssetStatus", "SiteInfrastructureAsset");
        Group("SoilZone", "SoilZone", "SoilZoneStatus", "SoilZoneBoundaryVersion");
        Group("SoilSample", "SoilSample", "SoilSampleResult", "SoilSampleStatus");
        Group("Common", "DigitalTwinText");
        Group("TraceEvent", "TraceObjectType", "TraceEventType", "TraceObjectRole", "TraceRelationshipType", "TraceEvent", "TraceEventObject", "TraceLink");
        Group("RecallCase", "RecallStatus", "RecallCase", "RecallSource");
        Group("QualityAssessment", "QualityAssessment"); Group("StorageLot", "TraceabilityMassBalance");
        Group("EpcisExchange", "EpcisExchangeDirection", "EpcisExchange", "EpcisCapturedEvent");
        Group("SupplyRelationship", "SupplyRelationshipStatus", "SupplyPartyRole", "SupplyRelationship");
        Group("DataGrant", "DataGrantStatus", "SupplyDataCategories", "DataGrant", "CrossTenantAccessAudit");
        Group("SupplierInvitation", "SupplierInvitationStatus", "SupplierInvitation");
        Group("HostedSupplierAccount", "HostedSupplierStatus", "HostedSupplierAccount", "HostedSupplierLinkEvent");
        Group("SupplyContract", "SupplyContractStatus", "ContractAllocationStatus", "SupplyContract", "ContractAllocation", "ContractPriceRule", "ContractPricingMethod", "ContractReconciliation", "ContractReconciliationStatus");
        Group("SupplierEvidenceRequest", "SupplierEvidenceRequestStatus", "SupplierEvidenceRequest");
        Group("SupplierRiskAssessment", "SupplierRiskBand", "SupplierRiskAssessment");
        Group("SupplierSelfServiceToken", "SupplierSelfServiceAction", "SupplierSelfServiceTokenStatus", "SupplierSelfServiceToken");
        Group("IdentityDevice", "IdentityDeviceStatus", "IdentityDevice"); Group("IdentitySession", "IdentitySessionStatus", "IdentitySession");
        Group("SecurityEvent", "SecurityEventOutcome", "SecurityEvent"); Group("AccountRecoveryRequest", "AccountRecoveryStatus", "AccountRecoveryRequest");
        Group("TenantAccount", "TenantAccountStatus", "TenantAccount"); Group("SaasPlanVersion", "SaasPlanStatus", "EntitlementKind", "EntitlementEnforcement", "SaasPlanVersion", "PlanEntitlement");
        Group("TenantSubscription", "BillingInterval", "SubscriptionStatus", "TenantSubscription"); Group("UsageMeterPeriod", "UsageMeterEvent", "UsageMeterPeriod");
        Group("BillingAccount", "BillingAccountStatus", "BillingAccount"); Group("BillingInvoice", "BillingInvoiceStatus", "BillingInvoice");
        Group("CropPlan", "CropPlanStatus", "PlannedResourceType", "FinancialEntryType", "CropPlan", "CropPlanAllocation", "PlannedCropOperation", "CropPlanResourceBudget", "CropPlanFinancialEntry");
        Group("PublicApiClient", "PublicApiClientStatus", "PublicApiCredentialStatus", "PublicApiScopes", "PublicApiClient", "PublicApiCredential", "PublicApiAccessLog");
        Group("WebhookSubscription", "WebhookStatus", "WebhookDeliveryStatus", "WebhookSubscription", "WebhookDelivery");
        Group("RetailDisclosurePolicy", "DisclosurePolicyStatus", "DisclosureAttributes", "RetailDisclosurePolicy");
        Group("TransparencyToken", "TransparencyTokenStatus", "TransparencyToken", "TransparencyAccessLog");
        Group("ApprovalPolicy", "ApprovalPolicyStatus", "ApprovalPolicy"); Group("DelegatedApprovalRequest", "DelegatedApprovalStatus", "ApprovalDecisionKind", "DelegatedApprovalRequest", "ApprovalDecision");
        Group("TenantPlacement", "TenantDatabaseMode", "TenantPlacement"); Group("TenantMigrationJob", "TenantMigrationKind", "TenantMigrationStatus", "TenantMigrationJob");
        Group("WorkOrder", "WorkOrderStatus", "WorkOrderPriority", "WorkResourceType", "WorkResourceStatus", "WorkResultType", "WorkOrder", "WorkOrderAssignment", "WorkOrderResource", "WorkOrderStatusHistory", "WorkOrderResult");
        Group("EudrDueDiligenceCase", "EudrCommodity", "EudrCaseStatus", "EudrScreeningResult", "EudrRiskConclusion", "EudrStatementStatus", "EudrDueDiligenceCase", "EudrPlotEvidence", "EudrDeforestationScreening", "EudrRiskDecision", "EudrDueDiligenceStatement", "EudrStatementExport");
        Group("LivestockStatutoryMovementDocument", "StatutoryMovementDocumentStatus", "StatutorySubmissionOutcome", "LivestockStatutoryMovementDocument", "LivestockStatutorySubmissionAttempt");
        Group("CropFeedTransfer", "CropFeedSourceType", "CropFeedTransfer"); Group("LivestockHolding", "LivestockHoldingType", "LivestockHolding");
        Group("LivestockSubject", "LivestockIdentifier", "LivestockGroupMembership", "LivestockSex", "LivestockBiologicalProfile", "LivestockMovementType", "LivestockMovementRecord", "LivestockHealthEventType", "LivestockHealthEvent");
        Group("LivestockReproductionEvent", "ReproductionEventType", "ReproductionOutcome", "OffspringOutcome", "LivestockReproductionEvent", "LivestockOffspringLink");
        Group("LivestockMilkRecord", "MilkSession", "LivestockMilkRecord"); Group("LivestockGrazingPlan", "GrazingPlanStatus", "LivestockGrazingPlan");
        Group("LivestockRationPlan", "RationPlanStatus", "LivestockRationPlan", "LivestockRationLine");
        Group("LivestockRegulatoryProfile", "LivestockRegulatoryExchangeType", "LivestockRegulatoryProfile");
        Group("LivestockExchangeSubmission", "LivestockExchangeSubmissionStatus", "LivestockExchangeAttemptOutcome", "LivestockExchangeSubmission", "LivestockExchangeAttempt");
        Group("LivestockFeedBatch", "LivestockFeedBatch", "LivestockFeedAllocation");
        Group("ComplianceStandardVersion", "ComplianceStandardStatus", "ComplianceControlApplicability", "ComplianceEvidenceType", "ComplianceStandardVersion", "ComplianceControlPoint", "ComplianceEvidenceRequirement");
        Group("ComplianceAssessment", "ComplianceAssessmentStatus", "ComplianceEvaluationResult", "ComplianceAssessment", "ComplianceControlEvaluation", "ComplianceEvidenceLink");
        Group("ComplianceFinding", "ComplianceFindingSeverity", "ComplianceFindingStatus", "CorrectiveActionStatus", "ComplianceFinding", "ComplianceCorrectiveAction");
        Group("ComplianceAuditPackExport", "ComplianceAuditPackFormat", "ComplianceAuditPackExport");
        Group("CostCentre", "CostCentreType", "CostCentre"); Group("FinanceTransaction", "FinanceEntryType", "FinanceSourceType", "CostAllocationMethod", "FinanceTransaction", "CostAllocation");
        Group("AccountingImportBatch", "AccountingImportStatus", "AccountingImportBatch", "AccountingStatementLine"); Group("AccountingReconciliation", "AccountingMatchStatus", "AccountingReconciliation");
        Group("IntercompanyEliminationRule", "EliminationRuleStatus", "IntercompanyEliminationRule"); Group("EnterpriseConsolidationRun", "ConsolidationEntryStatus", "EnterpriseConsolidationRun", "EnterpriseEliminationEntry");
        Group("WeatherLocation", "InsightQuality", "WeatherLocation", "WeatherObservation", "WeatherForecast"); Group("SatelliteFieldStatistic", "VegetationIndex", "SatelliteFieldStatistic");
        Group("ScoutingObservation", "ScoutingCategory", "InsightSeverity", "ScoutingObservation"); Group("FieldRiskRule", "RiskMetric", "RiskAggregation", "RiskComparison", "RiskRuleStatus", "FieldRiskRule", "FieldRiskAssessment");
        Group("FieldInsightAlert", "InsightAlertStatus", "FieldInsightAlert"); Group("FieldMapLayer", "MapLayerKind", "FieldMapLayer");
        Group("MasterDataImportJob", "MasterDataImportType", "MasterDataImportStatus", "MasterDataImportRowStatus", "MasterDataImportJob", "MasterDataImportRow");
        Group("GeospatialImportJob", "GeospatialImportFormat", "GeospatialImportStatus", "GeospatialImportJob", "GeospatialImportFeature");
        Group("AnalyticsProjectionRun", "AnalyticsProjectionStatus", "AnalyticsFactKind", "AnalyticsValueKind", "AnalyticsProjectionRun", "AnalyticsFact");
        Group("KpiDefinition", "KpiDefinitionStatus", "KpiDefinition"); Group("AnalyticsProjectionSchedule", "AnalyticsScheduleFrequency", "AnalyticsProjectionSchedule");
        Group("BenchmarkCohortDefinition", "AnalyticsGovernanceStatus", "BenchmarkCohortDefinition"); Group("AnalyticsDimensionSnapshot", "AnalyticsDimensionType", "AnalyticsDimensionSnapshot");
        Group("AnalyticsRefreshRequest", "AnalyticsRefreshRequestStatus", "AnalyticsRefreshRequest");
        Group("IntegrationConnection", "IntegrationCategory", "IntegrationConnectionStatus", "IntegrationHealth", "IntegrationCredentialStatus", "IntegrationConnection", "IntegrationCredential");
        Group("IntegrationFeed", "IntegrationFeedType", "IntegrationFeedStatus", "IntegrationTargetType", "IntegrationFeed");
        Group("IngestionJob", "IngestionJobKind", "IngestionJobStatus", "IngestionAttemptOutcome", "IngestionQuality", "IngestionRecordStatus", "IngestionJob", "IngestionAttempt", "IngestionRecord");
        Group("DerivedMediaAsset", "DerivedMediaKind", "DerivedMediaLifecycleStatus", "DerivedMediaAsset");
        Group("EquipmentAsset", "EquipmentType", "EquipmentStatus", "EquipmentAsset"); Group("ConnectedDevice", "ConnectedDeviceKind", "ConnectedDeviceProtocol", "ConnectedDeviceStatus", "ConnectedDevice", "SensorChannel");
        Group("SensorReading", "SensorValueQuality", "SensorReading"); Group("MachineryTaskFile", "MachineryTaskDirection", "MachineryTaskFormat", "ExchangeStatus", "MachineryTaskFile");
        Group("BusinessDocumentExchange", "BusinessDocumentType", "ExchangeDirection", "BusinessDocumentExchange");
        Group("Contact", "ContactType"); Group("LivestockProduction", "Gender", "LivestockType", "LivestockStatus", "LivestockUnitOfMeasure", "TagLocation", "TreatmentApplicationMethod", "TreatmentType");
    }

    sealed record Relocation(string Source, string Target, string OldNamespace, string NewNamespace, string Name, string Member, string Header);
    static void FinishDomain()
    {
        DomainOwners();
        string[] sources = Sources().ToArray();
        var original = sources.ToDictionary(Relative, File.ReadAllText);
        var changes = new List<Relocation>();
        foreach (var pair in original.Where(p => p.Key.StartsWith("src/Domain/Platform/") || p.Key == "src/Domain/Common/Enums.cs" || p.Key.StartsWith("src/Domain/Exceptions/")))
        {
            var root = Parse(pair.Value);
            var ns = root.Members.OfType<BaseNamespaceDeclarationSyntax>().Single();
            string header = string.Join("\n", root.Usings.Concat(ns.Usings).Select(u => u.ToString()));
            foreach (var member in ns.Members)
            {
                string name = TypeName(member);
                if (name.Length == 0) throw new InvalidOperationException($"Unsupported domain member in {pair.Key}");
                string owner = Owners.GetValueOrDefault(name, name);
                if (member is EnumDeclarationSyntax && !Owners.ContainsKey(name)) throw new InvalidOperationException($"Enum ownership missing: {name}");
                string target = pair.Key.StartsWith("src/Domain/Exceptions/") ? $"src/Application/Common/Exceptions/{name}.cs" : $"src/Domain/{owner}/{name}.cs";
                if (File.Exists(Full(target))) throw new InvalidOperationException($"Target exists: {target}");
                changes.Add(new(pair.Key, target, ns.Name.ToString(), NewNamespace(target), name, member.ToFullString(), header));
            }
        }
        if (changes.Count == 0) throw new InvalidOperationException("No pending domain bundles.");
        Backup("domain-completion");
        // Move each original into its first extracted declaration, then split the
        // remaining declarations; retain every declaration's original body/trivia.
        foreach (var group in changes.GroupBy(c => c.Source))
        {
            var first = group.First(); Move(first.Source, first.Target);
            foreach (var c in group)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Full(c.Target))!);
                File.WriteAllText(Full(c.Target), c.Header + "\n\nnamespace " + c.OldNamespace + ";\n" + c.Member);
            }
        }
        var map = changes.ToDictionary(c => c.OldNamespace + "." + c.Name, c => c.NewNamespace + "." + c.Name);
        var oldTypes = original.SelectMany(p => TopTypes(Parse(p.Value)).Select(t => (Ns: Namespace(t), Name: TypeName(t)))).Where(x => x.Ns.Length != 0).Distinct().ToArray();
        foreach (string path in Sources())
        {
            string rel = Relative(path), before = File.ReadAllText(path);
            var moved = changes.FirstOrDefault(c => c.Target == rel);
            string after = RewriteNamespaces(before, moved?.NewNamespace, map, oldTypes);
            if (before != after) File.WriteAllText(path, after);
        }
        File.WriteAllText(Full("output/refactoring/domain-completion/moves.json"), JsonSerializer.Serialize(changes.Select(c => new { c.Source, c.Target, c.Name }), Json));
        Console.WriteLine($"Split {changes.Select(c => c.Source).Distinct().Count()} bundles into {changes.Count} owned declarations.");
    }

    static string RewriteNamespaces(string text, string? newNamespace, Dictionary<string,string> map, (string Ns,string Name)[] allTypes)
    {
        var root = Parse(text);
        string oldNamespace = root.Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ?? "";
        var imports = root.Usings.Concat(root.Members.OfType<BaseNamespaceDeclarationSyntax>().SelectMany(n => n.Usings)).Where(u => u.Alias == null && u.StaticKeyword.IsKind(SyntaxKind.None)).Select(u => u.Name!.ToString()).ToHashSet();
        var declared = TopTypes(root).Select(TypeName).ToHashSet();
        var nsMap = allTypes.GroupBy(t => t.Ns).ToDictionary(g => g.Key, g => g.Select(t => map.GetValueOrDefault(t.Ns + "." + t.Name, t.Ns + "." + t.Name)).Select(n => n[..n.LastIndexOf('.')]).Distinct().ToArray());
        var edits = new List<(int Start, int Length, string Text)>();
        foreach (var u in root.DescendantNodes().OfType<UsingDirectiveSyntax>())
        {
            if (u.Alias == null && u.StaticKeyword.IsKind(SyntaxKind.None) && nsMap.TryGetValue(u.Name!.ToString(), out var destinations))
            {
                if (newNamespace?.StartsWith("EdgIQ.MurimiOS.Domain.") == true) destinations = destinations.Where(n => !n.StartsWith("EdgIQ.MurimiOS.Application.")).ToArray();
                string prefix = u.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword) ? "global using " : "using ";
                edits.Add((u.SpanStart,u.Span.Length,string.Join("\n",destinations.Select(n => prefix + n + ";"))));
            }
            else if (u.Alias != null && declared.Contains(u.Alias.Name.Identifier.ValueText)) edits.Add((u.SpanStart,u.Span.Length,""));
        }
        if (newNamespace != null)
            foreach (var ns in root.Members.OfType<BaseNamespaceDeclarationSyntax>()) edits.Add((ns.Name.SpanStart,ns.Name.Span.Length,newNamespace));
        foreach (var name in root.DescendantNodes().OfType<NameSyntax>().Where(n => n.Parent is not NameSyntax))
        {
            if (edits.Any(e => name.SpanStart >= e.Start && name.Span.End <= e.Start + e.Length)) continue;
            string value = name.ToString(), candidate = value.StartsWith("global::") ? value[8..] : value;
            if (map.TryGetValue(candidate, out string? updated)) edits.Add((name.SpanStart,name.Span.Length,(value.StartsWith("global::")?"global::":"")+updated));
        }
        foreach (var edit in edits.OrderByDescending(e => e.Start)) text = text.Remove(edit.Start,edit.Length).Insert(edit.Start,edit.Text);
        // Preserve implicit references to former siblings after aggregate splitting.
        if (newNamespace != null || oldNamespace.StartsWith("EdgIQ.MurimiOS.Domain."))
        {
            string destinationNs = newNamespace ?? oldNamespace;
            var current = Parse(text); var ns = current.Members.OfType<BaseNamespaceDeclarationSyntax>().SingleOrDefault();
            if (ns != null)
            {
                var aliases = current.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u => u.Alias != null).Select(u => u.Alias!.Name.Identifier.ValueText).ToHashSet();
                var tokens = ns.Members.SelectMany(m => m.DescendantTokens()).Where(t => t.IsKind(SyntaxKind.IdentifierToken)).Select(t => t.ValueText).ToHashSet();
                var add = new List<string>();
                foreach (var group in allTypes.Where(t => t.Ns == oldNamespace || oldNamespace.StartsWith(t.Ns + ".") || imports.Contains(t.Ns)).GroupBy(t => t.Name))
                {
                    if (declared.Contains(group.Key) || aliases.Contains(group.Key) || !tokens.Contains(group.Key)) continue;
                    var local = group.Where(t => t.Ns == oldNamespace).ToArray();
                    var visible = local.Length == 1 ? local : group.ToArray();
                    if (visible.Length != 1) continue;
                    string full = map.GetValueOrDefault(visible[0].Ns + "." + group.Key, visible[0].Ns + "." + group.Key);
                    GenericTypeNames ??= Sources().SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))))
                        .OfType<TypeDeclarationSyntax>().Where(t=>t.TypeParameterList!=null).Select(t=>Namespace(t)+"."+TypeName(t)).ToHashSet();
                    if(GenericTypeNames.Contains(full))continue;
                    if (full[..full.LastIndexOf('.')] == destinationNs || full.StartsWith("EdgIQ.MurimiOS.Domain.Common.")) continue;
                    add.Add("using " + group.Key + " = global::" + full + ";");
                }
                int offset = ns is FileScopedNamespaceDeclarationSyntax f ? f.SemicolonToken.Span.End : ((NamespaceDeclarationSyntax)ns).OpenBraceToken.Span.End;
                if (add.Count > 0) text = text.Insert(offset,"\n" + string.Join("\n",add));
            }
        }
        return text;
    }

    static void Inventory()
    {
        foreach (string file in Sources().Where(p => Relative(p).StartsWith("API/")))
        {
            var root = Parse(File.ReadAllText(file));
            var routes = root.DescendantNodes().OfType<InvocationExpressionSyntax>().Where(i => i.Expression is MemberAccessExpressionSyntax a && Regex.IsMatch(a.Name.Identifier.ValueText,"^Map(Get|Post|Put|Delete|Patch|Methods)$")).ToArray();
            if (routes.Length == 0) continue;
            Console.WriteLine(JsonSerializer.Serialize(new { File=Relative(file), Routes=routes.Select(r=>new { Call=r.Expression.ToString(), Args=r.ArgumentList.Arguments.Select(a=>a.Expression is LambdaExpressionSyntax?"lambda":a.ToString()).ToArray() }), HttpAccess=root.DescendantNodes().OfType<MemberAccessExpressionSyntax>().Select(n=>n.ToString()).Where(n=>Regex.IsMatch(n,@"^(context|httpContext|http|request|response)\.")).Distinct().ToArray() }));
        }
    }

    static void DeduplicateImports()
    {
        int count = 0;
        foreach (string file in Sources())
        {
            string before = File.ReadAllText(file); var root = Parse(before);
            var duplicates = root.DescendantNodes().OfType<UsingDirectiveSyntax>().GroupBy(u => u.Parent)
                .SelectMany(g => g.GroupBy(u => u.ToString()).SelectMany(s => s.Skip(1)))
                .Concat(root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u => u.Alias != null
                    && u.Parent is BaseNamespaceDeclarationSyntax ns && u.Name!.ToString().Replace("global::", "") == ns.Name + "." + u.Alias.Name))
                .Distinct().ToArray();
            if (duplicates.Length == 0) continue;
            string after = root.RemoveNodes(duplicates, SyntaxRemoveOptions.KeepExteriorTrivia)!.ToFullString();
            File.WriteAllText(file,after); count += duplicates.Length;
        }
        Console.WriteLine($"Removed {count} duplicate imports.");
    }

    static void FixCommonImports()
    {
        DomainOwners();
        using var archive = ZipFile.OpenRead(Full("output/refactoring/domain-completion/sources.zip"));
        using var moveDoc = JsonDocument.Parse(File.ReadAllText(Full("output/refactoring/domain-completion/moves.json")));
        var oldPaths = moveDoc.RootElement.EnumerateArray().ToDictionary(e=>e.GetProperty("Target").GetString()!,e=>e.GetProperty("Source").GetString()!);
        string[] enums = ["Gender","ContactType","LivestockType","LivestockStatus","LivestockUnitOfMeasure","TagLocation","TreatmentApplicationMethod","TreatmentType"];
        foreach (string file in Sources())
        {
            string rel = Relative(file); var entry = archive.GetEntry(oldPaths.GetValueOrDefault(rel,rel));
            if(entry == null) continue;
            using var reader = new StreamReader(entry.Open()); string original = reader.ReadToEnd(); var before = Parse(original);
            var oldImports = before.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias==null).Select(u=>u.Name!.ToString()).ToHashSet();
            if(!oldImports.Contains("EdgIQ.MurimiOS.Domain.Common")) continue;
            string text = File.ReadAllText(file); var root = Parse(text);
            var remove = root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias==null && u.Name?.ToString() is "EdgIQ.MurimiOS.Domain.Contact" or "EdgIQ.MurimiOS.Domain.LivestockProduction" && !oldImports.Contains(u.Name!.ToString())).ToArray();
            text=root.RemoveNodes(remove,SyntaxRemoveOptions.KeepExteriorTrivia)!.ToFullString(); root=Parse(text);
            bool global=before.Usings.Any(u=>u.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword) && u.Name?.ToString()=="EdgIQ.MurimiOS.Domain.Common");
            var declared=TopTypes(root).Select(TypeName).ToHashSet(); var aliases=root.DescendantNodes().OfType<UsingDirectiveSyntax>().Where(u=>u.Alias!=null).Select(u=>u.Alias!.Name.Identifier.ValueText).ToHashSet();
            var names=root.DescendantTokens().Where(t=>t.IsKind(SyntaxKind.IdentifierToken)).Select(t=>t.ValueText).ToHashSet();
            string additions=string.Join("\n",enums.Where(n=>!aliases.Contains(n)&&!declared.Contains(n)&&(global||names.Contains(n))).Select(n=>(global?"global ":"")+"using "+n+" = global::EdgIQ.MurimiOS.Domain."+Owners[n]+"."+n+";"));
            if(additions.Length!=0)
            {
                var ns=root.Members.OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
                int offset=global||ns==null?0:ns is FileScopedNamespaceDeclarationSyntax f?f.SemicolonToken.Span.End:((NamespaceDeclarationSyntax)ns).OpenBraceToken.Span.End;
                text=text.Insert(offset,"\n"+additions+"\n");
            }
            File.WriteAllText(file,text);
        }
        DeduplicateImports();
    }

    static void Dependencies()
    {
        var candidates=Sources().SelectMany(f=>TopTypes(Parse(File.ReadAllText(f))).Select(t=>new {Name=TypeName(t),File=Relative(f),Kind=t.Kind().ToString()}))
            .Where(t=>t.File.StartsWith("Infrastructure/")||t.File.StartsWith("API/Access/")||t.File.StartsWith("API/Tenancy/")||t.File=="API/Platform/IntegrationCredentialProtector.cs"||t.File=="API/Platform/SyncCommandProcessor.cs"||t.File=="API/Reporting/ProductionReportPdfBuilder.cs").ToArray();
        var api=Sources().Where(f=>Relative(f).StartsWith("API/")).Select(f=>new {File=Relative(f),Root=Parse(File.ReadAllText(f))}).Where(x=>x.Root.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(i=>i.Expression.ToString().Contains(".Map"))).ToArray();
        Console.WriteLine(JsonSerializer.Serialize(candidates.Select(c=>new {c.Name,c.File,c.Kind,Consumers=api.Where(a=>a.Root.DescendantTokens().Any(t=>t.IsKind(SyntaxKind.IdentifierToken)&&t.ValueText==c.Name)).Select(a=>a.File).ToArray()}).Where(c=>c.Consumers.Length>0),Json));
    }
}
