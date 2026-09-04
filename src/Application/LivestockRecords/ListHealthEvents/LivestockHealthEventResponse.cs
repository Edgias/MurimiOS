using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListHealthEvents;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;

using LivestockHealthEventType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEventType;
public sealed record LivestockHealthEventResponse(Guid Id, Guid SubjectId, LivestockHealthEventType Type, string Title, DateTimeOffset OccurredAt, string? Diagnosis, string? Practitioner, string? Notes);
