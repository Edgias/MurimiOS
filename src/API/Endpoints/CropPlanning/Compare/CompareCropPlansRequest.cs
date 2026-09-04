using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Compare;
public sealed record CompareCropPlansRequest(IReadOnlyList<Guid> PlanIds);
