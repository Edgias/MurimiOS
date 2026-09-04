using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar;
public sealed record OperationalCalendarItem(string Source, Guid Id, string? Number, string Title, string WorkType,
    string Status, DateTimeOffset Start, DateTimeOffset End, Guid? FieldId, Guid? CropZoneId,
    Guid? LivestockSubjectId, Guid? CropPlanId, Guid? PlannedOperationId, bool RequiresMaterialization);
