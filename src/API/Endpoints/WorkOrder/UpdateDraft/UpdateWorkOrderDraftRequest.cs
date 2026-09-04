using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WorkOrder;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.UpdateDraft;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
public sealed record UpdateWorkOrderDraftRequest(string Title, string WorkType, WorkOrderPriority Priority,
    DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string Instructions);
