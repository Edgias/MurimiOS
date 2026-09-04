using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.Performance;

using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
public sealed record LivestockPerformanceResponse(Guid SubjectId, string Name, LivestockSubjectKind Kind,
    DateTimeOffset From, DateTimeOffset To, decimal? StartingWeightKg, DateTimeOffset? StartingWeightAt,
    decimal? EndingWeightKg, DateTimeOffset? EndingWeightAt, decimal? WeightGainKg, decimal? DailyLiveWeightGainKg,
    decimal? ProjectedWeightKgAt30Days, decimal FeedAllocatedKg, decimal? FeedConversionRatio,
    decimal TotalMilkLitres, decimal SaleableMilkLitres);
