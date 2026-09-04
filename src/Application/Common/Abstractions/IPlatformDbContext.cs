using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
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

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;
namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
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
using WorkOrderAssignment = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderAssignment;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;

using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;

public interface IPlatformDbContext
{
    DbSet<OrganisationNode> OrganisationNodes { get; }
    DbSet<AuditEvent> AuditEvents { get; }
    DbSet<CorrectionRecord> CorrectionRecords { get; }
    DbSet<TenantMembership> TenantMemberships { get; }
    DbSet<MembershipPermissionGrant> MembershipPermissionGrants { get; }
    DbSet<Farm> Farms { get; }
    DbSet<FarmField> FarmFields { get; }
    DbSet<FieldBoundaryVersion> FieldBoundaryVersions { get; }
    DbSet<LandTenureRecord> LandTenureRecords { get; }
    DbSet<SiteInfrastructureAsset> SiteInfrastructureAssets { get; }
    DbSet<SoilZone> SoilZones { get; }
    DbSet<SoilZoneBoundaryVersion> SoilZoneBoundaryVersions { get; }
    DbSet<SoilSample> SoilSamples { get; }
    DbSet<SoilSampleResult> SoilSampleResults { get; }
    DbSet<GrowingSeason> GrowingSeasons { get; }
    DbSet<CropZone> CropZones { get; }
    DbSet<HarvestLot> HarvestLots { get; }
    DbSet<CropPlan> CropPlans { get; }
    DbSet<CropPlanAllocation> CropPlanAllocations { get; }
    DbSet<PlannedCropOperation> PlannedCropOperations { get; }
    DbSet<CropPlanResourceBudget> CropPlanResourceBudgets { get; }
    DbSet<CropProductApplicationRule> CropProductApplicationRules { get; }
    DbSet<CropPlanFinancialEntry> CropPlanFinancialEntries { get; }
    DbSet<CostCentre> CostCentres { get; }
    DbSet<FinanceTransaction> FinanceTransactions { get; }
    DbSet<FinanceOpeningBalance> FinanceOpeningBalances { get; }
    DbSet<CostAllocation> CostAllocations { get; }
    DbSet<LabourCostEntry> LabourCostEntries { get; }
    DbSet<MachineryCostEntry> MachineryCostEntries { get; }
    DbSet<AccountingImportBatch> AccountingImportBatches { get; }
    DbSet<AccountingStatementLine> AccountingStatementLines { get; }
    DbSet<AccountingReconciliation> AccountingReconciliations { get; }
    DbSet<LegalCompany> LegalCompanies { get; }
    DbSet<IntercompanyEliminationRule> IntercompanyEliminationRules { get; }
    DbSet<EnterpriseConsolidationRun> EnterpriseConsolidationRuns { get; }
    DbSet<EnterpriseEliminationEntry> EnterpriseEliminationEntries { get; }
    DbSet<StorageLot> StorageLots { get; }
    DbSet<TraceEvent> TraceEvents { get; }
    DbSet<TraceEventObject> TraceEventObjects { get; }
    DbSet<TraceLink> TraceLinks { get; }
    DbSet<EpcisExchange> EpcisExchanges { get; }
    DbSet<EpcisCapturedEvent> EpcisCapturedEvents { get; }
    DbSet<Delivery> Deliveries { get; }
    DbSet<DeliveryLine> DeliveryLines { get; }
    DbSet<RecallCase> RecallCases { get; }
    DbSet<RecallSource> RecallSources { get; }
    DbSet<QualityAssessment> QualityAssessments { get; }
    DbSet<SupplyRelationship> SupplyRelationships { get; }
    DbSet<DataGrant> DataGrants { get; }
    DbSet<CrossTenantAccessAudit> CrossTenantAccessAudits { get; }
    DbSet<ComplianceStandardVersion> ComplianceStandardVersions { get; }
    DbSet<ComplianceControlPoint> ComplianceControlPoints { get; }
    DbSet<ComplianceEvidenceRequirement> ComplianceEvidenceRequirements { get; }
    DbSet<ComplianceAssessment> ComplianceAssessments { get; }
    DbSet<ComplianceControlEvaluation> ComplianceControlEvaluations { get; }
    DbSet<ComplianceEvidenceLink> ComplianceEvidenceLinks { get; }
    DbSet<ComplianceFinding> ComplianceFindings { get; }
    DbSet<ComplianceCorrectiveAction> ComplianceCorrectiveActions { get; }
    DbSet<ComplianceAuditPackExport> ComplianceAuditPackExports { get; }
    DbSet<SupplierInvitation> SupplierInvitations { get; }
    DbSet<HostedSupplierAccount> HostedSupplierAccounts { get; }
    DbSet<SupplyContract> SupplyContracts { get; }
    DbSet<ContractAllocation> ContractAllocations { get; }
    DbSet<SupplierEvidenceRequest> SupplierEvidenceRequests { get; }
    DbSet<SupplierRiskAssessment> SupplierRiskAssessments { get; }
    DbSet<SupplierSelfServiceToken> SupplierSelfServiceTokens { get; }
    DbSet<HostedSupplierLinkEvent> HostedSupplierLinkEvents { get; }
    DbSet<ContractPriceRule> ContractPriceRules { get; }
    DbSet<ContractReconciliation> ContractReconciliations { get; }
    DbSet<AgriculturalInput> AgriculturalInputs { get; }
    DbSet<InputStockLot> InputStockLots { get; }
    DbSet<InventoryMovement> InventoryMovements { get; }
    DbSet<EvidenceAttachment> EvidenceAttachments { get; }
    DbSet<MediaUploadSession> MediaUploadSessions { get; }
    DbSet<ReportExportRecord> ReportExports { get; }
    DbSet<FieldOperationRecord> FieldOperations { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<WorkOrderAssignment> WorkOrderAssignments { get; }
    DbSet<WorkOrderResource> WorkOrderResources { get; }
    DbSet<WorkOrderStatusHistory> WorkOrderStatusHistory { get; }
    DbSet<WorkOrderResult> WorkOrderResults { get; }
    DbSet<IntegrationConnection> IntegrationConnections { get; }
    DbSet<EquipmentAsset> EquipmentAssets { get; }
    DbSet<ConnectedDevice> ConnectedDevices { get; }
    DbSet<SensorChannel> SensorChannels { get; }
    DbSet<SensorReading> SensorReadings { get; }
    DbSet<MachineryTelemetryRecord> MachineryTelemetry { get; }
    DbSet<MachineryTaskFile> MachineryTaskFiles { get; }
    DbSet<BusinessDocumentExchange> BusinessDocumentExchanges { get; }
    DbSet<DerivedMediaAsset> DerivedMediaAssets { get; }
    DbSet<KpiDefinition> KpiDefinitions { get; }
    DbSet<AnalyticsProjectionRun> AnalyticsProjectionRuns { get; }
    DbSet<AnalyticsFact> AnalyticsFacts { get; }
    DbSet<BenchmarkEnrollment> BenchmarkEnrollments { get; }
    DbSet<BenchmarkCohortDefinition> BenchmarkCohortDefinitions { get; }
    DbSet<PredictiveModelDefinition> PredictiveModelDefinitions { get; }
    DbSet<AnalyticsDimensionSnapshot> AnalyticsDimensionSnapshots { get; }
    DbSet<AnalyticsForecastEvaluation> AnalyticsForecastEvaluations { get; }
    DbSet<AnalyticsRefreshRequest> AnalyticsRefreshRequests { get; }
    DbSet<AnalyticsProjectionSchedule> AnalyticsProjectionSchedules { get; }
    DbSet<AnalyticsRetentionPolicy> AnalyticsRetentionPolicies { get; }
    DbSet<BenchmarkAggregateProjection> BenchmarkAggregates { get; }
    DbSet<PublicApiClient> PublicApiClients { get; }
    DbSet<PublicApiCredential> PublicApiCredentials { get; }
    DbSet<PublicApiAccessLog> PublicApiAccessLogs { get; }
    DbSet<WebhookSubscription> WebhookSubscriptions { get; }
    DbSet<WebhookDelivery> WebhookDeliveries { get; }
    DbSet<RetailDisclosurePolicy> RetailDisclosurePolicies { get; }
    DbSet<TransparencyToken> TransparencyTokens { get; }
    DbSet<TransparencyAccessLog> TransparencyAccessLogs { get; }
    DbSet<ApprovalPolicy> ApprovalPolicies { get; }
    DbSet<DelegatedApprovalRequest> DelegatedApprovalRequests { get; }
    DbSet<ApprovalDecision> ApprovalDecisions { get; }
    DbSet<TenantPlacement> TenantPlacements { get; }
    DbSet<TenantMigrationJob> TenantMigrationJobs { get; }
    DbSet<TenantAccount> TenantAccounts { get; }
    DbSet<SaasPlanVersion> SaasPlanVersions { get; }
    DbSet<PlanEntitlement> PlanEntitlements { get; }
    DbSet<TenantSubscription> TenantSubscriptions { get; }
    DbSet<UsageMeterEvent> UsageMeterEvents { get; }
    DbSet<UsageMeterPeriod> UsageMeterPeriods { get; }
    DbSet<BillingAccount> BillingAccounts { get; }
    DbSet<BillingInvoice> BillingInvoices { get; }
    DbSet<ApiRateLimitPolicy> ApiRateLimitPolicies { get; }
    DbSet<ApiCredentialRotationPolicy> ApiCredentialRotationPolicies { get; }
    DbSet<IdentityDevice> IdentityDevices { get; }
    DbSet<IdentitySession> IdentitySessions { get; }
    DbSet<SecurityEvent> SecurityEvents { get; }
    DbSet<AccountRecoveryRequest> AccountRecoveryRequests { get; }
    DbSet<IntegrationCredential> IntegrationCredentials { get; }
    DbSet<IntegrationFeed> IntegrationFeeds { get; }
    DbSet<IngestionJob> IngestionJobs { get; }
    DbSet<IngestionAttempt> IngestionAttempts { get; }
    DbSet<IngestionRecord> IngestionRecords { get; }
    DbSet<WeatherLocation> WeatherLocations { get; }
    DbSet<WeatherObservation> WeatherObservations { get; }
    DbSet<WeatherForecast> WeatherForecasts { get; }
    DbSet<SatelliteFieldStatistic> SatelliteFieldStatistics { get; }
    DbSet<ScoutingObservation> ScoutingObservations { get; }
    DbSet<FieldRiskRule> FieldRiskRules { get; }
    DbSet<FieldRiskAssessment> FieldRiskAssessments { get; }
    DbSet<FieldInsightAlert> FieldInsightAlerts { get; }
    DbSet<FieldMapLayer> FieldMapLayers { get; }
    DbSet<GeospatialImportJob> GeospatialImportJobs { get; }
    DbSet<GeospatialImportFeature> GeospatialImportFeatures { get; }
    DbSet<MasterDataImportJob> MasterDataImportJobs { get; }
    DbSet<MasterDataImportRow> MasterDataImportRows { get; }
    DbSet<LivestockSubject> LivestockSubjects { get; }
    DbSet<LivestockHolding> LivestockHoldings { get; }
    DbSet<LivestockIdentifier> LivestockIdentifiers { get; }
    DbSet<LivestockGroupMembership> LivestockGroupMemberships { get; }
    DbSet<LivestockMovementRecord> LivestockMovements { get; }
    DbSet<LivestockHealthEvent> LivestockHealthEvents { get; }
    DbSet<LivestockFeedBatch> LivestockFeedBatches { get; }
    DbSet<LivestockFeedAllocation> LivestockFeedAllocations { get; }
    DbSet<LivestockMeasurementRecord> LivestockMeasurements { get; }
    DbSet<VeterinaryMedicine> VeterinaryMedicines { get; }
    DbSet<VeterinaryMedicineStockLot> VeterinaryMedicineStockLots { get; }
    DbSet<LivestockTreatment> LivestockTreatments { get; }
    DbSet<LivestockDispatch> LivestockDispatches { get; }
    DbSet<LivestockBiologicalProfile> LivestockBiologicalProfiles { get; }
    DbSet<LivestockReproductionEvent> LivestockReproductionEvents { get; }
    DbSet<LivestockOffspringLink> LivestockOffspringLinks { get; }
    DbSet<LivestockMilkRecord> LivestockMilkRecords { get; }
    DbSet<LivestockGrazingPlan> LivestockGrazingPlans { get; }
    DbSet<LivestockRationPlan> LivestockRationPlans { get; }
    DbSet<LivestockRationLine> LivestockRationLines { get; }
    DbSet<LivestockStatutoryMovementDocument> LivestockStatutoryMovementDocuments { get; }
    DbSet<LivestockStatutorySubmissionAttempt> LivestockStatutorySubmissionAttempts { get; }
    DbSet<LivestockRegulatoryProfile> LivestockRegulatoryProfiles { get; }
    DbSet<LivestockExchangeSubmission> LivestockExchangeSubmissions { get; }
    DbSet<LivestockExchangeAttempt> LivestockExchangeAttempts { get; }
    DbSet<CropFeedTransfer> CropFeedTransfers { get; }
    DbSet<SyncCommandLog> SyncCommands { get; }
    DbSet<SyncChange> SyncChanges { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }
    DbSet<NotificationRecord> Notifications { get; }
    DbSet<NotificationPreference> NotificationPreferences { get; }
    DbSet<TenantRegionalSettings> TenantRegionalSettings { get; }
    DbSet<ReferenceCatalogue> ReferenceCatalogues { get; }
    DbSet<ReferenceCatalogueItem> ReferenceCatalogueItems { get; }
    DbSet<EudrDueDiligenceCase> EudrCases { get; }
    DbSet<EudrPlotEvidence> EudrPlotEvidence { get; }
    DbSet<EudrDeforestationScreening> EudrScreenings { get; }
    DbSet<EudrRiskDecision> EudrRiskDecisions { get; }
    DbSet<EudrDueDiligenceStatement> EudrStatements { get; }
    DbSet<EudrStatementExport> EudrStatementExports { get; }
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
