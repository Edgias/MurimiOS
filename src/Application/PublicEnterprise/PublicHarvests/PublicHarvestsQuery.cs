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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PublicHarvests;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiScopes = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiScopes;

public sealed record PublicHarvestsQuery(Guid? farmId) : IRequest<UseCaseResult>;
