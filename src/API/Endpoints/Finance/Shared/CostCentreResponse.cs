using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CostCentre;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.Shared;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
public sealed record CostCentreResponse(Guid Id, Guid FarmId, string Code, string Name,
    CostCentreType Type, Guid? TargetId, string Currency, bool IsActive,
    DateTimeOffset? ClosedAt, string? ClosedBy);
