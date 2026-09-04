using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateFeedAllocation;
public sealed record CreateLivestockFeedAllocationRequest(Guid? Id, Guid FeedBatchId, decimal Quantity, string Unit, DateTimeOffset OccurredAt, string? Notes);
