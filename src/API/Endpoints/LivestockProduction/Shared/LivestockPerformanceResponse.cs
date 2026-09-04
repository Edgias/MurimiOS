using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;

using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.Shared;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
public sealed record LivestockPerformanceResponse(Guid SubjectId, string Name, LivestockSubjectKind Kind,
    DateTimeOffset From, DateTimeOffset To, decimal? StartingWeightKg, DateTimeOffset? StartingWeightAt,
    decimal? EndingWeightKg, DateTimeOffset? EndingWeightAt, decimal? WeightGainKg, decimal? DailyLiveWeightGainKg,
    decimal? ProjectedWeightKgAt30Days, decimal FeedAllocatedKg, decimal? FeedConversionRatio,
    decimal TotalMilkLitres, decimal SaleableMilkLitres);
