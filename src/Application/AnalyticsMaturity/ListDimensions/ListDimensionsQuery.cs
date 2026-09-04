using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListDimensions;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsDimensionType = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionType;

public sealed record ListDimensionsQuery(Guid farmId, Guid? projectionRunId, AnalyticsDimensionType? type) : IRequest<UseCaseResult>;
