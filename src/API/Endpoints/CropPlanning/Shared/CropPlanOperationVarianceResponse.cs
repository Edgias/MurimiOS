using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record CropPlanOperationVarianceResponse(string OperationType, int PlannedCount, int ActualCount, int CountVariance);
