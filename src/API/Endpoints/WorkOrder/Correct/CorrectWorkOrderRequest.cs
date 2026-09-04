using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Correct;
public sealed record CorrectWorkOrderRequest(Guid? Id, string? Number, string Reason, string? Title = null,
    DateTimeOffset? ScheduledStart = null, DateTimeOffset? ScheduledEnd = null);
