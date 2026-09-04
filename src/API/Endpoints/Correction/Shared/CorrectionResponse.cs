using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Correction.Shared;

public sealed record CorrectionResponse(
    Guid Id,
    string EntityType,
    Guid OriginalEntityId,
    Guid ReplacementEntityId,
    string Reason,
    DateTimeOffset CorrectedAt,
    string CorrectedBy,
    Guid? FarmId);
