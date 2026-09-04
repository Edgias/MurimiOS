using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHolding;
using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;

public sealed record CreateLivestockHoldingRequest(Guid? Id, string Code, string Name, LivestockHoldingType Type);
