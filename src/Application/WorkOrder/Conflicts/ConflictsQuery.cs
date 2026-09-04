using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.WorkOrder.Conflicts;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;

public sealed record ConflictsQuery(Guid farmId, DateTimeOffset from, DateTimeOffset to, Guid? membershipId, string? resourceReference, Guid? excludeWorkOrderId) : IRequest<UseCaseResult>;
