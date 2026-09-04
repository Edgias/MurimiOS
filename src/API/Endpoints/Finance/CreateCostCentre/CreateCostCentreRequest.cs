using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CostCentre;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Finance.CreateCostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;

public sealed record CreateCostCentreRequest(Guid? Id, string Code, string Name,
    CostCentreType Type, Guid? TargetId, string Currency);
