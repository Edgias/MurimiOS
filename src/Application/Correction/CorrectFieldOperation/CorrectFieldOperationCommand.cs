using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.CorrectFieldOperation;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CorrectFieldOperationCommand(Guid operationId, CorrectFieldOperationRequest request) : IRequest<UseCaseResult>;
