using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Assign;
public sealed record AssignWorkOrderRequest(Guid? Id, Guid MembershipId, string AssignmentRole, bool IsPrimary);
