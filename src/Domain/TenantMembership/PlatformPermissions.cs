namespace EdgIQ.MurimiOS.Domain.TenantMembership;

public static class PlatformPermissions
{
    public const string AccessManage = "access.manage";
    public const string AuditRead = "audit.read";
    public const string FarmRead = "farm.read";
    public const string FarmWrite = "farm.write";
    public const string ProductionRead = "production.read";
    public const string ProductionWrite = "production.write";
    public const string InventoryRead = "inventory.read";
    public const string InventoryWrite = "inventory.write";
    public const string EvidenceRead = "evidence.read";
    public const string EvidenceWrite = "evidence.write";
    public const string LivestockRead = "livestock.read";
    public const string LivestockWrite = "livestock.write";
    public const string WithdrawalOverride = "livestock.withdrawal.override";
    public const string ReportingRead = "reporting.read";
    public const string SyncUse = "sync.use";
    public const string NotificationsRead = "notifications.read";
    public const string NotificationsManage = "notifications.manage";
    public const string SettingsRead = "settings.read";
    public const string SettingsManage = "settings.manage";
    public const string ReferenceDataRead = "reference-data.read";
    public const string ReferenceDataManage = "reference-data.manage";
    public const string TraceabilityRead = "traceability.read";
    public const string TraceabilityWrite = "traceability.write";
    public const string RecallManage = "traceability.recall.manage";
    public const string SupplyNetworkRead = "supply-network.read";
    public const string SupplyNetworkManage = "supply-network.manage";
    public const string DataGrantManage = "supply-network.grants.manage";
    public const string PlanningRead = "planning.read";
    public const string PlanningWrite = "planning.write";
    public const string PlanningApprove = "planning.approve";
    public const string FinanceRead = "finance.read";
    public const string FinanceWrite = "finance.write";
    public const string FinanceReconcile = "finance.reconcile";
    public const string OperationsRead = "operations.read";
    public const string OperationsWrite = "operations.write";
    public const string OperationsAssign = "operations.assign";
    public const string OperationsVerify = "operations.verify";
    public const string IntegrationsRead = "integrations.read";
    public const string IntegrationsManage = "integrations.manage";
    public const string IntegrationsOperate = "integrations.operate";
    public const string FieldIntelligenceRead = "field-intelligence.read";
    public const string FieldIntelligenceWrite = "field-intelligence.write";
    public const string FieldIntelligenceEvaluate = "field-intelligence.evaluate";
    public const string ImportsRead = "imports.read";
    public const string ImportsManage = "imports.manage";
    public const string ComplianceRead = "compliance.read";
    public const string ComplianceManage = "compliance.manage";
    public const string ComplianceAssess = "compliance.assess";
    public const string ComplianceVerify = "compliance.verify";
    public const string ComplianceExport = "compliance.export";
    public const string SupplierPortalRead = "supplier-portal.read";
    public const string SupplierPortalManage = "supplier-portal.manage";
    public const string ConnectedAgricultureRead = "connected-agriculture.read";
    public const string ConnectedAgricultureManage = "connected-agriculture.manage";
    public const string ConnectedAgricultureIngest = "connected-agriculture.ingest";
    public const string AnalyticsRead = "analytics.read";
    public const string AnalyticsManage = "analytics.manage";
    public const string AnalyticsBenchmark = "analytics.benchmark";
    public const string PublicApiRead = "public-api.read";
    public const string PublicApiManage = "public-api.manage";
    public const string RetailTransparencyRead = "retail-transparency.read";
    public const string RetailTransparencyManage = "retail-transparency.manage";
    public const string EnterpriseApprovalManage = "enterprise.approvals.manage";
    public const string EnterpriseApprove = "enterprise.approve";
    public const string TenantMigrationManage = "enterprise.tenant-migration.manage";
    public const string EudrRead = "eudr.read";
    public const string EudrManage = "eudr.manage";
    public const string EudrScreen = "eudr.screen";
    public const string EudrAssess = "eudr.assess";
    public const string EudrExport = "eudr.export";
    public const string SaasRead = "saas.read";
    public const string SaasManage = "saas.manage";
    public const string SaasBillingManage = "saas.billing.manage";
    public const string SaasUsageWrite = "saas.usage.write";
    public const string SecurityRead = "security.read";
    public const string SecurityManage = "security.manage";
    public const string AccountRecoveryManage = "security.recovery.manage";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.Ordinal)
    {
        AccessManage, AuditRead, FarmRead, FarmWrite, ProductionRead, ProductionWrite,
        InventoryRead, InventoryWrite, EvidenceRead, EvidenceWrite,
        LivestockRead, LivestockWrite, WithdrawalOverride, ReportingRead, SyncUse,
        NotificationsRead, NotificationsManage, SettingsRead, SettingsManage,
        ReferenceDataRead, ReferenceDataManage, TraceabilityRead, TraceabilityWrite, RecallManage,
        SupplyNetworkRead, SupplyNetworkManage, DataGrantManage,
        PlanningRead, PlanningWrite, PlanningApprove,
        FinanceRead, FinanceWrite, FinanceReconcile,
        OperationsRead, OperationsWrite, OperationsAssign, OperationsVerify,
        IntegrationsRead, IntegrationsManage, IntegrationsOperate,
        FieldIntelligenceRead, FieldIntelligenceWrite, FieldIntelligenceEvaluate,
        ImportsRead, ImportsManage,
        ComplianceRead, ComplianceManage, ComplianceAssess, ComplianceVerify, ComplianceExport,
        SupplierPortalRead, SupplierPortalManage,
        ConnectedAgricultureRead, ConnectedAgricultureManage, ConnectedAgricultureIngest,
        AnalyticsRead, AnalyticsManage, AnalyticsBenchmark,
        PublicApiRead, PublicApiManage, RetailTransparencyRead, RetailTransparencyManage,
        EnterpriseApprovalManage, EnterpriseApprove, TenantMigrationManage,
        EudrRead, EudrManage, EudrScreen, EudrAssess, EudrExport,
        SaasRead, SaasManage, SaasBillingManage, SaasUsageWrite,
        SecurityRead, SecurityManage, AccountRecoveryManage
    };

    public static IReadOnlySet<string> ForRole(TenantRole role) => role switch
    {
        TenantRole.Owner or TenantRole.Administrator => All,
        TenantRole.FarmManager => Set(FarmRead, FarmWrite, ProductionRead, ProductionWrite,
            InventoryRead, InventoryWrite, EvidenceRead, EvidenceWrite, LivestockRead,
            LivestockWrite, ReportingRead, SyncUse, NotificationsRead, SettingsRead, ReferenceDataRead,
            TraceabilityRead, TraceabilityWrite, RecallManage, SupplyNetworkRead, SupplyNetworkManage, DataGrantManage,
            PlanningRead, PlanningWrite, PlanningApprove, FinanceRead, FinanceWrite, FinanceReconcile,
            OperationsRead, OperationsWrite, OperationsAssign, OperationsVerify,
            IntegrationsRead, IntegrationsManage, IntegrationsOperate,
            FieldIntelligenceRead, FieldIntelligenceWrite, FieldIntelligenceEvaluate, ImportsRead, ImportsManage,
            ComplianceRead, ComplianceManage, ComplianceAssess, ComplianceVerify, ComplianceExport, SupplierPortalRead, SupplierPortalManage,
            ConnectedAgricultureRead, ConnectedAgricultureManage, ConnectedAgricultureIngest,
            AnalyticsRead, AnalyticsManage, AnalyticsBenchmark, PublicApiRead, PublicApiManage,
            RetailTransparencyRead, RetailTransparencyManage, EnterpriseApprovalManage, EnterpriseApprove,
            TenantMigrationManage, EudrRead, EudrManage, EudrScreen, EudrAssess, EudrExport),
        TenantRole.Agronomist => Set(FarmRead, ProductionRead, ProductionWrite, InventoryRead,
            InventoryWrite, EvidenceRead, EvidenceWrite, ReportingRead, SyncUse, NotificationsRead, SettingsRead, ReferenceDataRead,
            TraceabilityRead, TraceabilityWrite, SupplyNetworkRead, PlanningRead, PlanningWrite, FinanceRead,
            OperationsRead, OperationsWrite, OperationsAssign, IntegrationsRead,
            FieldIntelligenceRead, FieldIntelligenceWrite, FieldIntelligenceEvaluate, ImportsRead, ImportsManage,
            ComplianceRead, ComplianceAssess, SupplierPortalRead, ConnectedAgricultureRead,
            ConnectedAgricultureManage, ConnectedAgricultureIngest, AnalyticsRead, EnterpriseApprove,
            EudrRead, EudrManage, EudrScreen, EudrAssess),
        TenantRole.LivestockManager => Set(FarmRead, InventoryRead, EvidenceRead, EvidenceWrite,
            LivestockRead, LivestockWrite, ReportingRead, SyncUse, NotificationsRead, SettingsRead, ReferenceDataRead,
            TraceabilityRead, SupplyNetworkRead, FinanceRead, OperationsRead, OperationsWrite, OperationsAssign,
            IntegrationsRead, FieldIntelligenceRead, ImportsRead, ComplianceRead, ComplianceAssess, SupplierPortalRead,
            ConnectedAgricultureRead, ConnectedAgricultureManage, ConnectedAgricultureIngest, AnalyticsRead, EnterpriseApprove,
            EudrRead, EudrManage, EudrScreen),
        TenantRole.Operator => Set(FarmRead, ProductionRead, ProductionWrite, InventoryRead,
            EvidenceRead, EvidenceWrite, LivestockRead, LivestockWrite, SyncUse, NotificationsRead, SettingsRead, ReferenceDataRead,
            TraceabilityRead, TraceabilityWrite, SupplyNetworkRead, PlanningRead, OperationsRead, OperationsWrite,
            FieldIntelligenceRead, FieldIntelligenceWrite, ImportsRead, ComplianceRead, SupplierPortalRead,
            ConnectedAgricultureRead, AnalyticsRead, EudrRead),
        TenantRole.Auditor => Set(AuditRead, FarmRead, ProductionRead, InventoryRead, EvidenceRead,
            LivestockRead, ReportingRead, NotificationsRead, SettingsRead, ReferenceDataRead, TraceabilityRead, SupplyNetworkRead,
            PlanningRead, FinanceRead, OperationsRead, IntegrationsRead, FieldIntelligenceRead, ImportsRead,
            ComplianceRead, ComplianceAssess, ComplianceVerify, ComplianceExport, SupplierPortalRead, ConnectedAgricultureRead,
            AnalyticsRead, AnalyticsBenchmark, PublicApiRead, RetailTransparencyRead, EnterpriseApprove,
            EudrRead, EudrAssess, EudrExport, SecurityRead),
        TenantRole.Buyer => Set(FarmRead, ProductionRead, EvidenceRead, ReportingRead, NotificationsRead, SettingsRead, ReferenceDataRead,
            TraceabilityRead, SupplyNetworkRead, SupplyNetworkManage, ComplianceRead, SupplierPortalRead, SupplierPortalManage,
            AnalyticsRead, AnalyticsBenchmark, PublicApiRead, RetailTransparencyRead, RetailTransparencyManage,
            EudrRead, EudrManage, EudrScreen, EudrAssess, EudrExport),
        TenantRole.Viewer => Set(FarmRead, ProductionRead, InventoryRead, EvidenceRead,
            LivestockRead, ReportingRead, NotificationsRead, SettingsRead, ReferenceDataRead, TraceabilityRead, SupplyNetworkRead,
            PlanningRead, OperationsRead, FieldIntelligenceRead, ImportsRead, ComplianceRead, SupplierPortalRead,
            ConnectedAgricultureRead, AnalyticsRead, EudrRead),
        _ => new HashSet<string>(StringComparer.Ordinal)
    };

    private static IReadOnlySet<string> Set(params string[] permissions) =>
        new HashSet<string>(permissions, StringComparer.Ordinal);
}
