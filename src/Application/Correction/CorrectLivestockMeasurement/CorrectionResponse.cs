using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.CorrectLivestockMeasurement;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed record CorrectionResponse(
    Guid Id,
    string EntityType,
    Guid OriginalEntityId,
    Guid ReplacementEntityId,
    string Reason,
    DateTimeOffset CorrectedAt,
    string CorrectedBy,
    Guid? FarmId);
