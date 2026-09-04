using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
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
public sealed record MachineryCostResponse(Guid Id, Guid TransactionId, Guid FarmId, Guid CostCentreId,
    Guid? FieldOperationId, Guid? CropZoneId, Guid? LivestockSubjectId, string MachineReference,
    DateTimeOffset OccurredAt, decimal DurationHours, decimal DurationMinutes, decimal HourlyRate,
    decimal AdditionalCost, decimal TotalCost, string Currency, string? Notes);
