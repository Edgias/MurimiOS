using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Correction.List;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;

public sealed record CorrectionResponse(
    Guid Id,
    string EntityType,
    Guid OriginalEntityId,
    Guid ReplacementEntityId,
    string Reason,
    DateTimeOffset CorrectedAt,
    string CorrectedBy,
    Guid? FarmId);
