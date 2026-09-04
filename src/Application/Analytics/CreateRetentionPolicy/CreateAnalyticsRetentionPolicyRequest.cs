using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CreateRetentionPolicy;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;
public sealed record CreateAnalyticsRetentionPolicyRequest(Guid? Id, int RawSensorDays, int RawTelemetryDays,
    int FailedProjectionDays, DateTimeOffset NextRunAt, int DerivedMediaDays = 90, int ExpiredMapLayerDays = 90);
