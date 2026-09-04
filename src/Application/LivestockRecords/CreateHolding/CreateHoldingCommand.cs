using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateHolding;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;

public sealed record CreateHoldingCommand(Guid farmId, CreateLivestockHoldingRequest request) : IRequest<UseCaseResult>;
