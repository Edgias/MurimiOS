using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using EdgIQ.MurimiOS.Domain.TenantPlacement;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;

using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.LabourCostEntry;
using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using EdgIQ.MurimiOS.Domain.NotificationPreference;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using EdgIQ.MurimiOS.Domain.ReportExportRecord;
using EdgIQ.MurimiOS.Domain.TenantAccount;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy;
using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.Data;
using TenantEntity = global::EdgIQ.MurimiOS.Domain.Common.TenantEntity;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using AnalyticsRefreshRequest = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequest;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using ApprovalDecision = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecision;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using DeliveryLine = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryLine;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
using EudrStatementExport = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrStatementExport;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierLinkEvent = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierLinkEvent;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;
using LabourCostEntry = global::EdgIQ.MurimiOS.Domain.LabourCostEntry.LabourCostEntry;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using LivestockOffspringLink = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockOffspringLink;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using LivestockTreatment = global::EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using MachineryCostEntry = global::EdgIQ.MurimiOS.Domain.MachineryCostEntry.MachineryCostEntry;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;
using NotificationPreference = global::EdgIQ.MurimiOS.Domain.NotificationPreference.NotificationPreference;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;
using QualityAssessment = global::EdgIQ.MurimiOS.Domain.QualityAssessment.QualityAssessment;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using ReportExportRecord = global::EdgIQ.MurimiOS.Domain.ReportExportRecord.ReportExportRecord;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractReconciliation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using SyncCommandLog = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandLog;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;
using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TransparencyAccessLog = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyAccessLog;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;
using UsageMeterEvent = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterEvent;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;
using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderAssignment = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderAssignment;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
using BenchmarkAggregateProjection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.BenchmarkAggregateProjection;
using IPlatformDbContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class PlatformDbContext(
    DbContextOptions<PlatformDbContext> options,
    ITenantContext tenantContext) : DbContext(options)
,EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext{
    public DbSet<OrganisationNode> OrganisationNodes => Set<OrganisationNode>();

    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    public DbSet<CorrectionRecord> CorrectionRecords => Set<CorrectionRecord>();

    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    public DbSet<MembershipPermissionGrant> MembershipPermissionGrants => Set<MembershipPermissionGrant>();

    public DbSet<Farm> Farms => Set<Farm>();

    public DbSet<FarmField> FarmFields => Set<FarmField>();

    public DbSet<FieldBoundaryVersion> FieldBoundaryVersions => Set<FieldBoundaryVersion>();

    public DbSet<LandTenureRecord> LandTenureRecords => Set<LandTenureRecord>();

    public DbSet<SiteInfrastructureAsset> SiteInfrastructureAssets => Set<SiteInfrastructureAsset>();

    public DbSet<SoilZone> SoilZones => Set<SoilZone>();

    public DbSet<SoilZoneBoundaryVersion> SoilZoneBoundaryVersions => Set<SoilZoneBoundaryVersion>();

    public DbSet<SoilSample> SoilSamples => Set<SoilSample>();

    public DbSet<SoilSampleResult> SoilSampleResults => Set<SoilSampleResult>();

    public DbSet<GrowingSeason> GrowingSeasons => Set<GrowingSeason>();

    public DbSet<CropZone> CropZones => Set<CropZone>();

    public DbSet<HarvestLot> HarvestLots => Set<HarvestLot>();

    public DbSet<CropPlan> CropPlans => Set<CropPlan>();

    public DbSet<CropPlanAllocation> CropPlanAllocations => Set<CropPlanAllocation>();

    public DbSet<PlannedCropOperation> PlannedCropOperations => Set<PlannedCropOperation>();

    public DbSet<CropPlanResourceBudget> CropPlanResourceBudgets => Set<CropPlanResourceBudget>();

    public DbSet<CropProductApplicationRule> CropProductApplicationRules => Set<CropProductApplicationRule>();

    public DbSet<CropPlanFinancialEntry> CropPlanFinancialEntries => Set<CropPlanFinancialEntry>();

    public DbSet<CostCentre> CostCentres => Set<CostCentre>();

    public DbSet<FinanceTransaction> FinanceTransactions => Set<FinanceTransaction>();

    public DbSet<FinanceOpeningBalance> FinanceOpeningBalances => Set<FinanceOpeningBalance>();

    public DbSet<CostAllocation> CostAllocations => Set<CostAllocation>();

    public DbSet<LabourCostEntry> LabourCostEntries => Set<LabourCostEntry>();

    public DbSet<MachineryCostEntry> MachineryCostEntries => Set<MachineryCostEntry>();

    public DbSet<AccountingImportBatch> AccountingImportBatches => Set<AccountingImportBatch>();

    public DbSet<AccountingStatementLine> AccountingStatementLines => Set<AccountingStatementLine>();

    public DbSet<AccountingReconciliation> AccountingReconciliations => Set<AccountingReconciliation>();

    public DbSet<LegalCompany> LegalCompanies => Set<LegalCompany>();

    public DbSet<IntercompanyEliminationRule> IntercompanyEliminationRules => Set<IntercompanyEliminationRule>();

    public DbSet<EnterpriseConsolidationRun> EnterpriseConsolidationRuns => Set<EnterpriseConsolidationRun>();

    public DbSet<EnterpriseEliminationEntry> EnterpriseEliminationEntries => Set<EnterpriseEliminationEntry>();

    public DbSet<StorageLot> StorageLots => Set<StorageLot>();

    public DbSet<TraceEvent> TraceEvents => Set<TraceEvent>();

    public DbSet<TraceEventObject> TraceEventObjects => Set<TraceEventObject>();

    public DbSet<TraceLink> TraceLinks => Set<TraceLink>();

    public DbSet<EpcisExchange> EpcisExchanges => Set<EpcisExchange>();

    public DbSet<EpcisCapturedEvent> EpcisCapturedEvents => Set<EpcisCapturedEvent>();

    public DbSet<Delivery> Deliveries => Set<Delivery>();

    public DbSet<DeliveryLine> DeliveryLines => Set<DeliveryLine>();

    public DbSet<RecallCase> RecallCases => Set<RecallCase>();

    public DbSet<RecallSource> RecallSources => Set<RecallSource>();

    public DbSet<QualityAssessment> QualityAssessments => Set<QualityAssessment>();

    public DbSet<SupplyRelationship> SupplyRelationships => Set<SupplyRelationship>();

    public DbSet<DataGrant> DataGrants => Set<DataGrant>();

    public DbSet<CrossTenantAccessAudit> CrossTenantAccessAudits => Set<CrossTenantAccessAudit>();

    public DbSet<ComplianceStandardVersion> ComplianceStandardVersions => Set<ComplianceStandardVersion>();

    public DbSet<ComplianceControlPoint> ComplianceControlPoints => Set<ComplianceControlPoint>();

    public DbSet<ComplianceEvidenceRequirement> ComplianceEvidenceRequirements => Set<ComplianceEvidenceRequirement>();

    public DbSet<ComplianceAssessment> ComplianceAssessments => Set<ComplianceAssessment>();

    public DbSet<ComplianceControlEvaluation> ComplianceControlEvaluations => Set<ComplianceControlEvaluation>();

    public DbSet<ComplianceEvidenceLink> ComplianceEvidenceLinks => Set<ComplianceEvidenceLink>();

    public DbSet<ComplianceFinding> ComplianceFindings => Set<ComplianceFinding>();

    public DbSet<ComplianceCorrectiveAction> ComplianceCorrectiveActions => Set<ComplianceCorrectiveAction>();

    public DbSet<ComplianceAuditPackExport> ComplianceAuditPackExports => Set<ComplianceAuditPackExport>();

    public DbSet<SupplierInvitation> SupplierInvitations => Set<SupplierInvitation>();

    public DbSet<HostedSupplierAccount> HostedSupplierAccounts => Set<HostedSupplierAccount>();

    public DbSet<SupplyContract> SupplyContracts => Set<SupplyContract>();

    public DbSet<ContractAllocation> ContractAllocations => Set<ContractAllocation>();

    public DbSet<SupplierEvidenceRequest> SupplierEvidenceRequests => Set<SupplierEvidenceRequest>();

    public DbSet<SupplierRiskAssessment> SupplierRiskAssessments => Set<SupplierRiskAssessment>();

    public DbSet<SupplierSelfServiceToken> SupplierSelfServiceTokens => Set<SupplierSelfServiceToken>();

    public DbSet<HostedSupplierLinkEvent> HostedSupplierLinkEvents => Set<HostedSupplierLinkEvent>();

    public DbSet<ContractPriceRule> ContractPriceRules => Set<ContractPriceRule>();

    public DbSet<ContractReconciliation> ContractReconciliations => Set<ContractReconciliation>();

    public DbSet<AgriculturalInput> AgriculturalInputs => Set<AgriculturalInput>();

    public DbSet<InputStockLot> InputStockLots => Set<InputStockLot>();

    public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();

    public DbSet<EvidenceAttachment> EvidenceAttachments => Set<EvidenceAttachment>();

    public DbSet<MediaUploadSession> MediaUploadSessions => Set<MediaUploadSession>();

    public DbSet<ReportExportRecord> ReportExports => Set<ReportExportRecord>();

    public DbSet<FieldOperationRecord> FieldOperations => Set<FieldOperationRecord>();

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    public DbSet<WorkOrderAssignment> WorkOrderAssignments => Set<WorkOrderAssignment>();

    public DbSet<WorkOrderResource> WorkOrderResources => Set<WorkOrderResource>();

    public DbSet<WorkOrderStatusHistory> WorkOrderStatusHistory => Set<WorkOrderStatusHistory>();

    public DbSet<WorkOrderResult> WorkOrderResults => Set<WorkOrderResult>();

    public DbSet<IntegrationConnection> IntegrationConnections => Set<IntegrationConnection>();

    public DbSet<EquipmentAsset> EquipmentAssets => Set<EquipmentAsset>();

    public DbSet<ConnectedDevice> ConnectedDevices => Set<ConnectedDevice>();

    public DbSet<SensorChannel> SensorChannels => Set<SensorChannel>();

    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();

    public DbSet<MachineryTelemetryRecord> MachineryTelemetry => Set<MachineryTelemetryRecord>();

    public DbSet<MachineryTaskFile> MachineryTaskFiles => Set<MachineryTaskFile>();

    public DbSet<BusinessDocumentExchange> BusinessDocumentExchanges => Set<BusinessDocumentExchange>();

    public DbSet<DerivedMediaAsset> DerivedMediaAssets => Set<DerivedMediaAsset>();

    public DbSet<KpiDefinition> KpiDefinitions => Set<KpiDefinition>();

    public DbSet<AnalyticsProjectionRun> AnalyticsProjectionRuns => Set<AnalyticsProjectionRun>();

    public DbSet<AnalyticsFact> AnalyticsFacts => Set<AnalyticsFact>();

    public DbSet<BenchmarkEnrollment> BenchmarkEnrollments => Set<BenchmarkEnrollment>();
    public DbSet<BenchmarkCohortDefinition> BenchmarkCohortDefinitions => Set<BenchmarkCohortDefinition>();
    public DbSet<PredictiveModelDefinition> PredictiveModelDefinitions => Set<PredictiveModelDefinition>();
    public DbSet<AnalyticsDimensionSnapshot> AnalyticsDimensionSnapshots => Set<AnalyticsDimensionSnapshot>();
    public DbSet<AnalyticsForecastEvaluation> AnalyticsForecastEvaluations => Set<AnalyticsForecastEvaluation>();
    public DbSet<AnalyticsRefreshRequest> AnalyticsRefreshRequests => Set<AnalyticsRefreshRequest>();
    public DbSet<AnalyticsProjectionSchedule> AnalyticsProjectionSchedules => Set<AnalyticsProjectionSchedule>();
    public DbSet<AnalyticsRetentionPolicy> AnalyticsRetentionPolicies => Set<AnalyticsRetentionPolicy>();

    public DbSet<BenchmarkAggregateProjection> BenchmarkAggregates => Set<BenchmarkAggregateProjection>();

    public DbSet<PublicApiClient> PublicApiClients => Set<PublicApiClient>();
    public DbSet<PublicApiCredential> PublicApiCredentials => Set<PublicApiCredential>();
    public DbSet<PublicApiAccessLog> PublicApiAccessLogs => Set<PublicApiAccessLog>();
    public DbSet<WebhookSubscription> WebhookSubscriptions => Set<WebhookSubscription>();
    public DbSet<WebhookDelivery> WebhookDeliveries => Set<WebhookDelivery>();
    public DbSet<RetailDisclosurePolicy> RetailDisclosurePolicies => Set<RetailDisclosurePolicy>();
    public DbSet<TransparencyToken> TransparencyTokens => Set<TransparencyToken>();
    public DbSet<TransparencyAccessLog> TransparencyAccessLogs => Set<TransparencyAccessLog>();
    public DbSet<ApprovalPolicy> ApprovalPolicies => Set<ApprovalPolicy>();
    public DbSet<DelegatedApprovalRequest> DelegatedApprovalRequests => Set<DelegatedApprovalRequest>();
    public DbSet<ApprovalDecision> ApprovalDecisions => Set<ApprovalDecision>();
    public DbSet<TenantPlacement> TenantPlacements => Set<TenantPlacement>();
    public DbSet<TenantMigrationJob> TenantMigrationJobs => Set<TenantMigrationJob>();

    public DbSet<TenantAccount> TenantAccounts => Set<TenantAccount>();
    public DbSet<SaasPlanVersion> SaasPlanVersions => Set<SaasPlanVersion>();
    public DbSet<PlanEntitlement> PlanEntitlements => Set<PlanEntitlement>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();
    public DbSet<UsageMeterEvent> UsageMeterEvents => Set<UsageMeterEvent>();
    public DbSet<UsageMeterPeriod> UsageMeterPeriods => Set<UsageMeterPeriod>();
    public DbSet<BillingAccount> BillingAccounts => Set<BillingAccount>();
    public DbSet<BillingInvoice> BillingInvoices => Set<BillingInvoice>();
    public DbSet<ApiRateLimitPolicy> ApiRateLimitPolicies => Set<ApiRateLimitPolicy>();
    public DbSet<ApiCredentialRotationPolicy> ApiCredentialRotationPolicies => Set<ApiCredentialRotationPolicy>();
    public DbSet<IdentityDevice> IdentityDevices => Set<IdentityDevice>();
    public DbSet<IdentitySession> IdentitySessions => Set<IdentitySession>();
    public DbSet<SecurityEvent> SecurityEvents => Set<SecurityEvent>();
    public DbSet<AccountRecoveryRequest> AccountRecoveryRequests => Set<AccountRecoveryRequest>();

    public DbSet<IntegrationCredential> IntegrationCredentials => Set<IntegrationCredential>();

    public DbSet<IntegrationFeed> IntegrationFeeds => Set<IntegrationFeed>();

    public DbSet<IngestionJob> IngestionJobs => Set<IngestionJob>();

    public DbSet<IngestionAttempt> IngestionAttempts => Set<IngestionAttempt>();

    public DbSet<IngestionRecord> IngestionRecords => Set<IngestionRecord>();

    public DbSet<WeatherLocation> WeatherLocations => Set<WeatherLocation>();

    public DbSet<WeatherObservation> WeatherObservations => Set<WeatherObservation>();

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    public DbSet<SatelliteFieldStatistic> SatelliteFieldStatistics => Set<SatelliteFieldStatistic>();

    public DbSet<ScoutingObservation> ScoutingObservations => Set<ScoutingObservation>();

    public DbSet<FieldRiskRule> FieldRiskRules => Set<FieldRiskRule>();

    public DbSet<FieldRiskAssessment> FieldRiskAssessments => Set<FieldRiskAssessment>();

    public DbSet<FieldInsightAlert> FieldInsightAlerts => Set<FieldInsightAlert>();

    public DbSet<FieldMapLayer> FieldMapLayers => Set<FieldMapLayer>();

    public DbSet<GeospatialImportJob> GeospatialImportJobs => Set<GeospatialImportJob>();

    public DbSet<GeospatialImportFeature> GeospatialImportFeatures => Set<GeospatialImportFeature>();

    public DbSet<MasterDataImportJob> MasterDataImportJobs => Set<MasterDataImportJob>();

    public DbSet<MasterDataImportRow> MasterDataImportRows => Set<MasterDataImportRow>();

    public DbSet<LivestockSubject> LivestockSubjects => Set<LivestockSubject>();

    public DbSet<LivestockHolding> LivestockHoldings => Set<LivestockHolding>();

    public DbSet<LivestockIdentifier> LivestockIdentifiers => Set<LivestockIdentifier>();

    public DbSet<LivestockGroupMembership> LivestockGroupMemberships => Set<LivestockGroupMembership>();

    public DbSet<LivestockMovementRecord> LivestockMovements => Set<LivestockMovementRecord>();

    public DbSet<LivestockHealthEvent> LivestockHealthEvents => Set<LivestockHealthEvent>();

    public DbSet<LivestockFeedBatch> LivestockFeedBatches => Set<LivestockFeedBatch>();

    public DbSet<LivestockFeedAllocation> LivestockFeedAllocations => Set<LivestockFeedAllocation>();

    public DbSet<LivestockMeasurementRecord> LivestockMeasurements => Set<LivestockMeasurementRecord>();

    public DbSet<VeterinaryMedicine> VeterinaryMedicines => Set<VeterinaryMedicine>();

    public DbSet<VeterinaryMedicineStockLot> VeterinaryMedicineStockLots => Set<VeterinaryMedicineStockLot>();

    public DbSet<LivestockTreatment> LivestockTreatments => Set<LivestockTreatment>();

    public DbSet<LivestockDispatch> LivestockDispatches => Set<LivestockDispatch>();

    public DbSet<LivestockBiologicalProfile> LivestockBiologicalProfiles => Set<LivestockBiologicalProfile>();

    public DbSet<LivestockReproductionEvent> LivestockReproductionEvents => Set<LivestockReproductionEvent>();

    public DbSet<LivestockOffspringLink> LivestockOffspringLinks => Set<LivestockOffspringLink>();

    public DbSet<LivestockMilkRecord> LivestockMilkRecords => Set<LivestockMilkRecord>();

    public DbSet<LivestockGrazingPlan> LivestockGrazingPlans => Set<LivestockGrazingPlan>();

    public DbSet<LivestockRationPlan> LivestockRationPlans => Set<LivestockRationPlan>();

    public DbSet<LivestockRationLine> LivestockRationLines => Set<LivestockRationLine>();

    public DbSet<LivestockStatutoryMovementDocument> LivestockStatutoryMovementDocuments => Set<LivestockStatutoryMovementDocument>();

    public DbSet<LivestockStatutorySubmissionAttempt> LivestockStatutorySubmissionAttempts => Set<LivestockStatutorySubmissionAttempt>();

    public DbSet<LivestockRegulatoryProfile> LivestockRegulatoryProfiles => Set<LivestockRegulatoryProfile>();

    public DbSet<LivestockExchangeSubmission> LivestockExchangeSubmissions => Set<LivestockExchangeSubmission>();

    public DbSet<LivestockExchangeAttempt> LivestockExchangeAttempts => Set<LivestockExchangeAttempt>();

    public DbSet<CropFeedTransfer> CropFeedTransfers => Set<CropFeedTransfer>();

    public DbSet<SyncCommandLog> SyncCommands => Set<SyncCommandLog>();

    public DbSet<SyncChange> SyncChanges => Set<SyncChange>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();

    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    public DbSet<TenantRegionalSettings> TenantRegionalSettings => Set<TenantRegionalSettings>();

    public DbSet<ReferenceCatalogue> ReferenceCatalogues => Set<ReferenceCatalogue>();

    public DbSet<ReferenceCatalogueItem> ReferenceCatalogueItems => Set<ReferenceCatalogueItem>();

    public DbSet<EudrDueDiligenceCase> EudrCases => Set<EudrDueDiligenceCase>();

    public DbSet<EudrPlotEvidence> EudrPlotEvidence => Set<EudrPlotEvidence>();

    public DbSet<EudrDeforestationScreening> EudrScreenings => Set<EudrDeforestationScreening>();

    public DbSet<EudrRiskDecision> EudrRiskDecisions => Set<EudrRiskDecision>();

    public DbSet<EudrDueDiligenceStatement> EudrStatements => Set<EudrDueDiligenceStatement>();

    public DbSet<EudrStatementExport> EudrStatementExports => Set<EudrStatementExport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMembership.TenantMembershipConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMembership.MembershipPermissionGrantConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AuditEvent.AuditEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CorrectionRecord.CorrectionRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.OrganisationNode.OrganisationNodeConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Farm.FarmConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FarmField.FarmFieldConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FarmField.FieldBoundaryVersionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LandTenureRecord.LandTenureRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SiteInfrastructureAsset.SiteInfrastructureAssetConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilZone.SoilZoneConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilZone.SoilZoneBoundaryVersionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilSample.SoilSampleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilSample.SoilSampleResultConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.GrowingSeason.GrowingSeasonConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropZone.CropZoneConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.HarvestLot.HarvestLotConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan.CropPlanConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan.CropPlanAllocationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan.PlannedCropOperationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan.CropPlanResourceBudgetConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropProductApplicationRule.CropProductApplicationRuleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan.CropPlanFinancialEntryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LegalCompany.LegalCompanyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IntercompanyEliminationRule.IntercompanyEliminationRuleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EnterpriseConsolidationRun.EnterpriseConsolidationRunConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EnterpriseConsolidationRun.EnterpriseEliminationEntryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CostCentre.CostCentreConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceTransaction.FinanceTransactionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceOpeningBalance.FinanceOpeningBalanceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceTransaction.CostAllocationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LabourCostEntry.LabourCostEntryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryCostEntry.MachineryCostEntryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingImportBatch.AccountingImportBatchConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingImportBatch.AccountingStatementLineConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingReconciliation.AccountingReconciliationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationConnection.IntegrationConnectionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationConnection.IntegrationCredentialConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationFeed.IntegrationFeedConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob.IngestionJobConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob.IngestionAttemptConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob.IngestionRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EquipmentAsset.EquipmentAssetConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ConnectedDevice.ConnectedDeviceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ConnectedDevice.SensorChannelConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SensorReading.SensorReadingConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryTelemetryRecord.MachineryTelemetryRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryTaskFile.MachineryTaskFileConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.BusinessDocumentExchange.BusinessDocumentExchangeConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.DerivedMediaAsset.DerivedMediaAssetConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.KpiDefinition.KpiDefinitionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionRun.AnalyticsProjectionRunConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionRun.AnalyticsFactConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.BenchmarkEnrollment.BenchmarkEnrollmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.BenchmarkCohortDefinition.BenchmarkCohortDefinitionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.PredictiveModelDefinition.PredictiveModelDefinitionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshotConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsForecastEvaluation.AnalyticsForecastEvaluationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsRefreshRequest.AnalyticsRefreshRequestConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionSchedule.AnalyticsProjectionScheduleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsRetentionPolicy.AnalyticsRetentionPolicyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections.BenchmarkAggregateProjectionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient.PublicApiClientConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient.PublicApiCredentialConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient.PublicApiAccessLogConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WebhookSubscription.WebhookSubscriptionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WebhookSubscription.WebhookDeliveryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.RetailDisclosurePolicy.RetailDisclosurePolicyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TransparencyToken.TransparencyTokenConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TransparencyToken.TransparencyAccessLogConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ApprovalPolicy.ApprovalPolicyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.DelegatedApprovalRequest.DelegatedApprovalRequestConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.DelegatedApprovalRequest.ApprovalDecisionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantPlacement.TenantPlacementConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMigrationJob.TenantMigrationJobConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantAccount.TenantAccountConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SaasPlanVersion.SaasPlanVersionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SaasPlanVersion.PlanEntitlementConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantSubscription.TenantSubscriptionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.UsageMeterPeriod.UsageMeterEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.UsageMeterPeriod.UsageMeterPeriodConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.BillingAccount.BillingAccountConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.BillingInvoice.BillingInvoiceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ApiRateLimitPolicy.ApiRateLimitPolicyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ApiCredentialRotationPolicy.ApiCredentialRotationPolicyConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IdentityDevice.IdentityDeviceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.IdentitySession.IdentitySessionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SecurityEvent.SecurityEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountRecoveryRequest.AccountRecoveryRequestConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WeatherLocation.WeatherLocationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WeatherLocation.WeatherObservationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WeatherLocation.WeatherForecastConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SatelliteFieldStatistic.SatelliteFieldStatisticConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ScoutingObservation.ScoutingObservationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldRiskRule.FieldRiskRuleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldRiskRule.FieldRiskAssessmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldInsightAlert.FieldInsightAlertConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldMapLayer.FieldMapLayerConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.GeospatialImportJob.GeospatialImportJobConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.GeospatialImportJob.GeospatialImportFeatureConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MasterDataImportJob.MasterDataImportJobConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MasterDataImportJob.MasterDataImportRowConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.StorageLot.StorageLotConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent.TraceEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent.TraceEventObjectConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent.TraceLinkConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EpcisExchange.EpcisExchangeConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EpcisExchange.EpcisCapturedEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Delivery.DeliveryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Delivery.DeliveryLineConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.RecallCase.RecallCaseConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.RecallCase.RecallSourceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.QualityAssessment.QualityAssessmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyRelationship.SupplyRelationshipConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.DataGrant.DataGrantConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.DataGrant.CrossTenantAccessAuditConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections.GrantedHarvestLotProjectionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections.GrantedTraceLinkProjectionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierInvitation.SupplierInvitationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.HostedSupplierAccount.HostedSupplierAccountConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract.SupplyContractConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract.ContractAllocationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierEvidenceRequest.SupplierEvidenceRequestConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierRiskAssessment.SupplierRiskAssessmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierSelfServiceToken.SupplierSelfServiceTokenConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.HostedSupplierAccount.HostedSupplierLinkEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract.ContractPriceRuleConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract.ContractReconciliationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion.ComplianceStandardVersionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion.ComplianceControlPointConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion.ComplianceEvidenceRequirementConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment.ComplianceAssessmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment.ComplianceControlEvaluationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment.ComplianceEvidenceLinkConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceFinding.ComplianceFindingConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceFinding.ComplianceCorrectiveActionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAuditPackExport.ComplianceAuditPackExportConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrDueDiligenceCaseConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrPlotEvidenceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrDeforestationScreeningConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrRiskDecisionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrDueDiligenceStatementConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase.EudrStatementExportConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldOperationRecord.FieldOperationRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder.WorkOrderConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder.WorkOrderAssignmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder.WorkOrderResourceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder.WorkOrderStatusHistoryConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder.WorkOrderResultConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.AgriculturalInput.AgriculturalInputConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.InputStockLot.InputStockLotConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.InventoryMovement.InventoryMovementConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.EvidenceAttachment.EvidenceAttachmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.MediaUploadSession.MediaUploadSessionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ReportExportRecord.ReportExportRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockSubjectConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockHolding.LivestockHoldingConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockIdentifierConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockGroupMembershipConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockMovementRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockHealthEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockFeedBatch.LivestockFeedBatchConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockFeedBatch.LivestockFeedAllocationConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockMeasurementRecord.LivestockMeasurementRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.VeterinaryMedicine.VeterinaryMedicineConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.VeterinaryMedicineStockLot.VeterinaryMedicineStockLotConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockTreatment.LivestockTreatmentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockDispatch.LivestockDispatchConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject.LivestockBiologicalProfileConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockReproductionEvent.LivestockReproductionEventConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockReproductionEvent.LivestockOffspringLinkConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockMilkRecord.LivestockMilkRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockGrazingPlan.LivestockGrazingPlanConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRationPlan.LivestockRationPlanConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRationPlan.LivestockRationLineConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocumentConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRegulatoryProfile.LivestockRegulatoryProfileConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttemptConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockExchangeSubmission.LivestockExchangeSubmissionConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockExchangeSubmission.LivestockExchangeAttemptConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.CropFeedTransfer.CropFeedTransferConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SyncClient.SyncCommandLogConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.SyncClient.SyncChangeConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.NotificationRecord.NotificationRecordConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.NotificationPreference.NotificationPreferenceConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantRegionalSettings.TenantRegionalSettingsConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ReferenceCatalogue.ReferenceCatalogueConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.ReferenceCatalogue.ReferenceCatalogueItemConfiguration.Configure(modelBuilder, this);
        global::EdgIQ.MurimiOS.Infrastructure.Data.Config.OutboxMessage.OutboxMessageConfiguration.Configure(modelBuilder, this);
    }

    internal static void ConfigureOperationalCostRelationships<TEntity>(
        Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> entry)
        where TEntity : TenantEntity
    {
        entry.HasOne<Farm>().WithMany()
            .HasForeignKey("TenantId", "FarmId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
        entry.HasOne<FinanceTransaction>().WithMany()
            .HasForeignKey("TenantId", "TransactionId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
        entry.HasOne<CostCentre>().WithMany()
            .HasForeignKey("TenantId", "CostCentreId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
        entry.HasOne<FieldOperationRecord>().WithMany()
            .HasForeignKey("TenantId", "FieldOperationId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
        entry.HasOne<CropZone>().WithMany()
            .HasForeignKey("TenantId", "CropZoneId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
        entry.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey("TenantId", "LivestockSubjectId")
            .HasPrincipalKey(nameof(TenantEntity.TenantId), nameof(TenantEntity.Id)).OnDelete(DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampTenantEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampTenantEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    private void StampTenantEntities()
    {
        if (!tenantContext.IsResolved || tenantContext.TenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A tenant context is required for platform data access.");
        }

        DateTimeOffset now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State is EntityState.Modified or EntityState.Deleted
                && entry.Entity is CorrectionRecord or ReportExportRecord or StorageLot or TraceEvent
                    or TraceEventObject or TraceLink or DeliveryLine or RecallSource or QualityAssessment
                    or LivestockMovementRecord or LivestockHealthEvent or LivestockFeedAllocation
                    or LivestockTreatment or LivestockDispatch or CropProductApplicationRule
                    or CropPlanFinancialEntry or FinanceTransaction or CostAllocation
                    or LabourCostEntry or MachineryCostEntry or AccountingStatementLine
                    or AccountingReconciliation or ComplianceControlEvaluation or ComplianceEvidenceLink
                    or SensorReading or MachineryTelemetryRecord or AnalyticsFact or PublicApiAccessLog
                    or TransparencyAccessLog or ApprovalDecision or UsageMeterEvent or SecurityEvent
                    or ComplianceAuditPackExport or SoilSampleResult or FinanceOpeningBalance
                    or EpcisExchange or EpcisCapturedEvent or SupplierRiskAssessment
                    or HostedSupplierLinkEvent or ContractPriceRule or ContractReconciliation
                    or EnterpriseConsolidationRun or EnterpriseEliminationEntry)
                throw new InvalidOperationException($"{entry.Metadata.ClrType.Name} records are append-only.");
            if (entry.State is EntityState.Modified or EntityState.Deleted
                && entry.Entity is LivestockStatutorySubmissionAttempt or CropFeedTransfer
                    or LivestockRegulatoryProfile or LivestockExchangeAttempt)
                throw new InvalidOperationException($"{entry.Metadata.ClrType.Name} records are append-only.");
            if (entry.State is EntityState.Modified or EntityState.Deleted
                && entry.Entity is AnalyticsDimensionSnapshot or AnalyticsForecastEvaluation or AnalyticsRefreshRequest)
                throw new InvalidOperationException($"{entry.Metadata.ClrType.Name} records are append-only or maintenance-managed.");
            if (entry.State == EntityState.Deleted
                && entry.Entity is FieldOperationRecord or LivestockMeasurementRecord or LivestockIdentifier
                    or LivestockGroupMembership or LivestockFeedBatch or CropPlan or CostCentre
                    or AccountingImportBatch or LivestockStatutoryMovementDocument or LandTenureRecord
                    or SiteInfrastructureAsset or SoilZone or SoilZoneBoundaryVersion or SoilSample
                    or MasterDataImportJob or MasterDataImportRow or SupplierSelfServiceToken
                    or LivestockExchangeSubmission or BenchmarkCohortDefinition or PredictiveModelDefinition
                    or LegalCompany or IntercompanyEliminationRule)
                throw new InvalidOperationException($"{entry.Metadata.ClrType.Name} records cannot be deleted; create a correction instead.");

            if (entry.State == EntityState.Modified && entry.Entity is MasterDataImportJob)
            {
                string[] allowed = [nameof(MasterDataImportJob.Status), nameof(MasterDataImportJob.RowCount),
                    nameof(MasterDataImportJob.ReadyRowCount), nameof(MasterDataImportJob.ErrorRowCount),
                    nameof(MasterDataImportJob.AppliedRowCount), nameof(MasterDataImportJob.FailedRowCount),
                    nameof(MasterDataImportJob.CheckpointOrdinal), nameof(MasterDataImportJob.ValidatedAt),
                    nameof(MasterDataImportJob.ApplyStartedAt), nameof(MasterDataImportJob.AppliedAt),
                    nameof(MasterDataImportJob.AppliedBy), nameof(MasterDataImportJob.FailureCode),
                    nameof(MasterDataImportJob.FailureDetail)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Master-data import source identity is immutable.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is MasterDataImportRow)
            {
                string[] allowed = [nameof(MasterDataImportRow.Status), nameof(MasterDataImportRow.AppliedEntityType),
                    nameof(MasterDataImportRow.AppliedEntityId), nameof(MasterDataImportRow.AppliedFarmId),
                    nameof(MasterDataImportRow.AppliedAt), nameof(MasterDataImportRow.FailureCode),
                    nameof(MasterDataImportRow.FailureDetail)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Validated master-data import row payloads are immutable.");
            }

            if (entry.State == EntityState.Modified && entry.Entity is LandTenureRecord)
            {
                string[] allowed = [nameof(LandTenureRecord.ValidTo), nameof(LandTenureRecord.Status),
                    nameof(LandTenureRecord.ClosedAt), nameof(LandTenureRecord.ClosedBy)];
                string[] forbidden = entry.Properties.Where(x => x.IsModified && !allowed.Contains(x.Metadata.Name))
                    .Select(x => x.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("Land-tenure history may only be closed or superseded.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is SoilZoneBoundaryVersion)
            {
                string[] forbidden = entry.Properties.Where(x => x.IsModified && x.Metadata.Name != nameof(SoilZoneBoundaryVersion.ValidTo))
                    .Select(x => x.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A soil-zone boundary version may only be closed.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is SiteInfrastructureAsset)
            {
                string[] allowed = [nameof(SiteInfrastructureAsset.Status), nameof(SiteInfrastructureAsset.DecommissionedOn),
                    nameof(SiteInfrastructureAsset.DecommissionedAt), nameof(SiteInfrastructureAsset.DecommissionedBy),
                    nameof(SiteInfrastructureAsset.DecommissionReason)];
                string[] forbidden = entry.Properties.Where(x => x.IsModified && !allowed.Contains(x.Metadata.Name))
                    .Select(x => x.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("Infrastructure identity and site details are immutable.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is SoilZone)
            {
                string[] allowed = [nameof(SoilZone.Status), nameof(SoilZone.RetiredAt), nameof(SoilZone.RetiredBy)];
                string[] forbidden = entry.Properties.Where(x => x.IsModified && !allowed.Contains(x.Metadata.Name))
                    .Select(x => x.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("Soil-zone identity is immutable after creation.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is SoilSample)
            {
                string[] allowed = [nameof(SoilSample.Status), nameof(SoilSample.SubmittedAt), nameof(SoilSample.AnalysedAt),
                    nameof(SoilSample.RejectedAt), nameof(SoilSample.RejectionReason)];
                string[] forbidden = entry.Properties.Where(x => x.IsModified && !allowed.Contains(x.Metadata.Name))
                    .Select(x => x.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("Soil-sample chain-of-custody fields are immutable.");
            }
            if (entry.State == EntityState.Modified
                && entry.Entity is FieldOperationRecord or LivestockMeasurementRecord)
            {
                string[] forbidden = entry.Properties
                    .Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(FieldOperationRecord.SupersededById)
                        && property.Metadata.Name is not nameof(FieldOperationRecord.CorrectionReason))
                    .Select(property => property.Metadata.Name)
                    .ToArray();
                if (forbidden.Length > 0)
                    throw new InvalidOperationException($"Completed records are immutable; forbidden changes: {string.Join(", ", forbidden)}.");
            }

            if (entry.State == EntityState.Modified && entry.Entity is LivestockIdentifier)
            {
                string[] forbidden = entry.Properties.Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(LivestockIdentifier.ValidTo))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A livestock identifier may only be retired.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is LivestockGroupMembership)
            {
                string[] forbidden = entry.Properties.Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(LivestockGroupMembership.LeftAt))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A livestock group membership may only be closed.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is LivestockFeedBatch)
            {
                string[] forbidden = entry.Properties.Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(LivestockFeedBatch.QuantityOnHand))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A feed batch may only be issued through allocation.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is LivestockStatutoryMovementDocument)
            {
                string[] allowed = [nameof(LivestockStatutoryMovementDocument.Status), nameof(LivestockStatutoryMovementDocument.SubmittedAt),
                    nameof(LivestockStatutoryMovementDocument.DecidedAt), nameof(LivestockStatutoryMovementDocument.ExternalReference),
                    nameof(LivestockStatutoryMovementDocument.RejectionCode), nameof(LivestockStatutoryMovementDocument.RejectionMessage)];
                string[] forbidden = entry.Properties.Where(property => property.IsModified && !allowed.Contains(property.Metadata.Name))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A statutory document snapshot is immutable after creation.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is LivestockExchangeSubmission)
            {
                string[] allowed = [nameof(LivestockExchangeSubmission.Status),
                    nameof(LivestockExchangeSubmission.FirstSubmittedAt), nameof(LivestockExchangeSubmission.DecidedAt),
                    nameof(LivestockExchangeSubmission.ExternalReference), nameof(LivestockExchangeSubmission.RejectionCode),
                    nameof(LivestockExchangeSubmission.RejectionMessage)];
                string[] forbidden = entry.Properties.Where(property => property.IsModified && !allowed.Contains(property.Metadata.Name))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("An external livestock submission snapshot is immutable after creation.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is BenchmarkCohortDefinition)
            {
                string[] allowed = [nameof(BenchmarkCohortDefinition.Status), nameof(BenchmarkCohortDefinition.ApprovedAt),
                    nameof(BenchmarkCohortDefinition.ApprovedBy), nameof(BenchmarkCohortDefinition.ApprovalReference),
                    nameof(BenchmarkCohortDefinition.RetiredAt), nameof(BenchmarkCohortDefinition.RetiredBy)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Benchmark cohort definitions are immutable after creation except for approval and retirement.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is PredictiveModelDefinition)
            {
                string[] allowed = [nameof(PredictiveModelDefinition.Status), nameof(PredictiveModelDefinition.ApprovedAt),
                    nameof(PredictiveModelDefinition.ApprovedBy), nameof(PredictiveModelDefinition.ApprovalReference),
                    nameof(PredictiveModelDefinition.RetiredAt), nameof(PredictiveModelDefinition.RetiredBy)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Predictive model definitions are immutable after creation except for approval and retirement.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is LegalCompany)
            {
                string[] allowed = [nameof(LegalCompany.IsActive), nameof(LegalCompany.EffectiveTo),
                    nameof(LegalCompany.ClosedAt), nameof(LegalCompany.ClosedBy)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Legal-company identity is immutable after registration.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is IntercompanyEliminationRule)
            {
                string[] allowed = [nameof(IntercompanyEliminationRule.Status), nameof(IntercompanyEliminationRule.ApprovedAt),
                    nameof(IntercompanyEliminationRule.ApprovedBy), nameof(IntercompanyEliminationRule.ApprovalReference),
                    nameof(IntercompanyEliminationRule.RetiredAt), nameof(IntercompanyEliminationRule.RetiredBy)];
                if (entry.Properties.Any(x => x.IsModified && !allowed.Contains(x.Metadata.Name)))
                    throw new InvalidOperationException("Elimination rules are immutable after creation except for approval and retirement.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is CostCentre)
            {
                string[] forbidden = entry.Properties.Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(CostCentre.IsActive)
                        && property.Metadata.Name is not nameof(CostCentre.ClosedAt)
                        && property.Metadata.Name is not nameof(CostCentre.ClosedBy))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("A cost centre may only be closed.");
            }
            if (entry.State == EntityState.Modified && entry.Entity is AccountingImportBatch)
            {
                string[] forbidden = entry.Properties.Where(property => property.IsModified
                        && property.Metadata.Name is not nameof(AccountingImportBatch.Status)
                        && property.Metadata.Name is not nameof(AccountingImportBatch.ReconciledAt)
                        && property.Metadata.Name is not nameof(AccountingImportBatch.ReconciledBy))
                    .Select(property => property.Metadata.Name).ToArray();
                if (forbidden.Length > 0) throw new InvalidOperationException("An accounting import may only complete reconciliation.");
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.AttachToTenant(tenantContext.TenantId, now);
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entry.Entity.TenantId != tenantContext.TenantId)
                {
                    throw new InvalidOperationException("Cross-tenant updates are forbidden.");
                }

                entry.Entity.MarkUpdated(now);
            }
        }

        foreach (var entry in ChangeTracker.Entries<SyncChange>().Where(x => x.State == EntityState.Added))
        {
            if (entry.Entity.TenantId != tenantContext.TenantId)
            {
                throw new InvalidOperationException("Cross-tenant sync changes are forbidden.");
            }
        }

        foreach (var entry in ChangeTracker.Entries<OutboxMessage>().Where(x => x.State == EntityState.Added))
        {
            if (entry.Entity.TenantId != tenantContext.TenantId)
            {
                throw new InvalidOperationException("Cross-tenant outbox messages are forbidden.");
            }
        }

        if (ChangeTracker.Entries<AuditEvent>().Any(entry => entry.State is EntityState.Modified or EntityState.Deleted))
            throw new InvalidOperationException("Audit events are append-only.");

        if (ChangeTracker.Entries<CrossTenantAccessAudit>().Any(entry => entry.State is EntityState.Modified or EntityState.Deleted))
            throw new InvalidOperationException("Cross-tenant access audit records are append-only.");

        foreach (var entry in ChangeTracker.Entries<SupplyRelationship>())
        {
            if (entry.State == EntityState.Added && entry.Entity.RequestedByTenantId != tenantContext.TenantId)
                throw new InvalidOperationException("A supply relationship request must be created by the current tenant.");
            if (entry.State == EntityState.Modified
                && entry.Entity.ProducerTenantId != tenantContext.TenantId
                && entry.Entity.BuyerTenantId != tenantContext.TenantId)
                throw new InvalidOperationException("Only a supply relationship party can change its status.");
            if (entry.State == EntityState.Deleted)
                throw new InvalidOperationException("Supply relationships are retained and cannot be deleted.");
        }

        foreach (var entry in ChangeTracker.Entries<DataGrant>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified
                && entry.Entity.ProducerTenantId != tenantContext.TenantId)
                throw new InvalidOperationException("Only the producer can create or revoke a data grant.");
            if (entry.State == EntityState.Deleted)
                throw new InvalidOperationException("Data grants are retained and cannot be deleted.");
        }

        foreach (var entry in ChangeTracker.Entries<CrossTenantAccessAudit>().Where(x => x.State == EntityState.Added))
        {
            if (entry.Entity.ActorTenantId != tenantContext.TenantId)
                throw new InvalidOperationException("Cross-tenant access must be audited as the current tenant.");
        }
    }

    internal void ConfigureTenantEntity<TEntity>(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TEntity> builder)
        where TEntity : TenantEntity
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Version).IsConcurrencyToken();
        builder.HasIndex(x => x.TenantId);
        builder.HasQueryFilter(x => x.TenantId == tenantContext.TenantId);
    }

    internal void ConfigureAuditEventFilter(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AuditEvent> builder)
        => builder.HasQueryFilter(x => x.TenantId == tenantContext.TenantId);

    internal void ConfigureSyncChangeFilter(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<SyncChange> builder)
        => builder.HasQueryFilter(x => x.TenantId == tenantContext.TenantId);

    internal void ConfigureOutboxMessageFilter(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OutboxMessage> builder)
        => builder.HasQueryFilter(x => x.TenantId == tenantContext.TenantId);
}
