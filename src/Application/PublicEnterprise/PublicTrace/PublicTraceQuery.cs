using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PublicTrace;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;

using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiScopes = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiScopes;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed record PublicTraceQuery(Guid harvestLotId) : IRequest<UseCaseResult>;
