using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
public sealed record WorkOrderAssignmentResponse(Guid Id, Guid MembershipId, string AssignmentRole, bool IsPrimary,
    DateTimeOffset AssignedAt, string AssignedBy, DateTimeOffset? UnassignedAt, string? UnassignedBy);
