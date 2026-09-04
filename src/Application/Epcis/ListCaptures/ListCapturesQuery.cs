using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.ListCaptures;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;

public sealed record ListCapturesQuery(Guid farmId, int? perPage) : IRequest<UseCaseResult>;
