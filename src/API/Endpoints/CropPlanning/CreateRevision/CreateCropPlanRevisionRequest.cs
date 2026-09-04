using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateRevision;
public sealed record CreateCropPlanRevisionRequest(Guid? Id, string? Name);
