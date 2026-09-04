using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar;
public sealed record OperationalCalendarResponse(Guid FarmId, string TimeZone, DateTimeOffset From, DateTimeOffset To,
    IReadOnlyList<OperationalCalendarItem> Items);
