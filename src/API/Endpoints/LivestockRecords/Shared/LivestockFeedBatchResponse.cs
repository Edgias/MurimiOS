using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.Shared;
public sealed record LivestockFeedBatchResponse(Guid Id, Guid FarmId, string BatchCode, string FeedName, decimal InitialQuantity, decimal QuantityOnHand, string Unit, decimal EnteredInitialQuantity, string EnteredUnit, DateOnly ReceivedOn, DateOnly? ExpiresOn, string? Supplier);
