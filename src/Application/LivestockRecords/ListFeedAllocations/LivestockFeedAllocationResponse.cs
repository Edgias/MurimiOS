using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListFeedAllocations;

public sealed record LivestockFeedAllocationResponse(Guid Id, Guid SubjectId, Guid FeedBatchId, decimal Quantity, string Unit, decimal EnteredQuantity, string EnteredUnit, DateTimeOffset OccurredAt, string? Notes);
