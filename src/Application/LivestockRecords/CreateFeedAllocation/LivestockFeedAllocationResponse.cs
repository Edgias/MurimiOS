using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.CreateFeedAllocation;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using MeasurementUnits = global::EdgIQ.MurimiOS.Domain.Common.MeasurementUnits;

using MeasuredQuantity = global::EdgIQ.MurimiOS.Domain.Common.MeasuredQuantity;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
public sealed record LivestockFeedAllocationResponse(Guid Id, Guid SubjectId, Guid FeedBatchId, decimal Quantity, string Unit, decimal EnteredQuantity, string EnteredUnit, DateTimeOffset OccurredAt, string? Notes);
