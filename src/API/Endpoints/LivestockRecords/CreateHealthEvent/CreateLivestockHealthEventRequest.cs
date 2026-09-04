using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHealthEvent;
using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
public sealed record CreateLivestockHealthEventRequest(Guid? Id, LivestockHealthEventType Type, string Title, DateTimeOffset OccurredAt, string? Diagnosis, string? Practitioner, string? Notes);
