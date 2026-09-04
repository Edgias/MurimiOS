using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.WorkOrder;

public sealed class WorkOrderResult : TenantEntity
{
    private WorkOrderResult() { }
    public WorkOrderResult(Guid id, Guid workOrderId, WorkResultType resultType, Guid resultId,
        string? notes, DateTimeOffset recordedAt, string recordedBy) : base(id)
    {
        if (workOrderId == Guid.Empty || resultId == Guid.Empty) throw new ArgumentException("Work order and result identifiers are required.");
        if (!Enum.IsDefined(resultType)) throw new ArgumentOutOfRangeException(nameof(resultType));
        WorkOrderId = workOrderId; ResultType = resultType; ResultId = resultId;
        Notes = Optional(notes, 2_000); RecordedAt = recordedAt.ToUniversalTime(); RecordedBy = Text(recordedBy, nameof(recordedBy), 240);
    }
    public Guid WorkOrderId { get; private set; }
    public WorkResultType ResultType { get; private set; }
    public Guid ResultId { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string RecordedBy { get; private set; } = string.Empty;
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
