using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateRation;
public sealed record CreateRationPlanRequest(string Name, DateOnly EffectiveFrom, DateOnly? EffectiveTo,
    int TargetHeadCount, string? Notes, IReadOnlyList<CreateRationLineRequest> Lines);
