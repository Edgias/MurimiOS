using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.CreateQualityAssessment;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using QualityAssessment = global::EdgIQ.MurimiOS.Domain.QualityAssessment.QualityAssessment;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
public sealed record QualityAssessmentResponse(Guid Id, TraceObjectType ObjectType, Guid ObjectId, string Metric,
    decimal? NumericResult, string? TextResult, string? Unit, bool Passed, DateTimeOffset AssessedAt, string? Notes);
