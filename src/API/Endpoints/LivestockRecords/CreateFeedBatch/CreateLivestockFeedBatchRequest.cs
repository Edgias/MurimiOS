using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedBatch;
public sealed record CreateLivestockFeedBatchRequest(Guid? Id, string BatchCode, string FeedName, decimal Quantity, string Unit, DateOnly ReceivedOn, DateOnly? ExpiresOn, string? Supplier);
