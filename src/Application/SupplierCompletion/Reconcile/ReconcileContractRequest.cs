using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.Reconcile;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractReconciliation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliation;
using ContractReconciliationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;
public sealed record ReconcileContractRequest(Guid? Id, string Reference, decimal AcceptedQuantity,
    decimal RejectedQuantity, string? QualityGrade, decimal QualityAdjustmentAmount,
    decimal OtherAdjustmentAmount, DateOnly? EffectiveOn, string? Notes);
