using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListRefreshRequests;
using AnalyticsRefreshRequest = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequest;
using AnalyticsRefreshRequestStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequestStatus;

public sealed record ListRefreshRequestsQuery(AnalyticsRefreshRequestStatus? status) : IRequest<UseCaseResult>;
