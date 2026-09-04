using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.Shared;
public sealed record LivestockFeedAllocationResponse(Guid Id, Guid SubjectId, Guid FeedBatchId, decimal Quantity, string Unit, decimal EnteredQuantity, string EnteredUnit, DateTimeOffset OccurredAt, string? Notes);
