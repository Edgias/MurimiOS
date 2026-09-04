using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMovement;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
public sealed record CreateLivestockMovementRequest(Guid? Id, LivestockMovementType Type, Guid? FromHoldingId, Guid? ToHoldingId, DateTimeOffset OccurredAt, string? Reference, string? Notes);
