using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.FarmPerformance;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;


using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using ReproductionEventType = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionEventType;
using ReproductionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionOutcome;
using LivestockSubjectKind = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubjectKind;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

public sealed record FarmPerformanceQuery(Guid farmId, DateTimeOffset? from, DateTimeOffset? to) : IRequest<UseCaseResult>;
