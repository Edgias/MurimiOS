using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using WorkResultType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResultType;
public sealed record WorkOrderResultResponse(Guid Id, WorkResultType ResultType, Guid ResultId, string? Notes,
    DateTimeOffset RecordedAt, string RecordedBy);
