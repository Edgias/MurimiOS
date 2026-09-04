using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CostCentre;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Finance.ListCostCentres;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostCentreType = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentreType;
public sealed record CostCentreResponse(Guid Id, Guid FarmId, string Code, string Name,
    CostCentreType Type, Guid? TargetId, string Currency, bool IsActive,
    DateTimeOffset? ClosedAt, string? ClosedBy);
