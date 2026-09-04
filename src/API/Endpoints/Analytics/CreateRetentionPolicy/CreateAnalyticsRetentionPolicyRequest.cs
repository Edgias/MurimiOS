
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateRetentionPolicy;
public sealed record CreateAnalyticsRetentionPolicyRequest(Guid? Id, int RawSensorDays, int RawTelemetryDays,
    int FailedProjectionDays, DateTimeOffset NextRunAt, int DerivedMediaDays = 90, int ExpiredMapLayerDays = 90);
