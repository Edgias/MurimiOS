using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.CorrectFieldOperation;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CorrectFieldOperationRequest(
    Guid? CorrectionId,
    Guid? ReplacementId,
    Guid? CommandId,
    string OperationType,
    DateTimeOffset OccurredAt,
    string? Notes,
    string Reason);
