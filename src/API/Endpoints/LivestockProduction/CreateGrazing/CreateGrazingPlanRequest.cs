using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateGrazing;
public sealed record CreateGrazingPlanRequest(Guid GroupSubjectId, Guid HoldingId, DateTimeOffset StartsAt,
    DateTimeOffset EndsAt, int TargetHeadCount, DateTimeOffset? RestUntil, decimal? AvailableDryMatterKg, string? Notes);
