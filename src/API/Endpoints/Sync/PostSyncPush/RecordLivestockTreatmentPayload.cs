using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record RecordLivestockTreatmentPayload(
    Guid SubjectId,
    Guid MedicineStockLotId,
    decimal Dose,
    string Route,
    DateTimeOffset AdministeredAt,
    string? Notes);
