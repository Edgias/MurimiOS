using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.QueryReadings;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;

public sealed record QueryReadingsQuery(Guid farmId, Guid? deviceId, string? metricCode, DateTimeOffset? from, DateTimeOffset? to, int? limit) : IRequest<UseCaseResult>;
