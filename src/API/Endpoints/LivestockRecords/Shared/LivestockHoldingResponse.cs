using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.Shared;
using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;
public sealed record LivestockHoldingResponse(Guid Id, Guid FarmId, string Code, string Name, LivestockHoldingType Type);
