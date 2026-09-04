using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreatePlan;

public sealed record CreateCropPlanRequest(Guid? Id, Guid SeasonId, Guid? PlanningGroupId,
    string Name, string ScenarioName, string Currency);
