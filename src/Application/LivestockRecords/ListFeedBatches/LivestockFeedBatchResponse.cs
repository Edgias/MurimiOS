using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListFeedBatches;

public sealed record LivestockFeedBatchResponse(Guid Id, Guid FarmId, string BatchCode, string FeedName, decimal InitialQuantity, decimal QuantityOnHand, string Unit, decimal EnteredInitialQuantity, string EnteredUnit, DateOnly ReceivedOn, DateOnly? ExpiresOn, string? Supplier);
