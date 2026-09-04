using System.Text.Json;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Notification.Get;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;

using NotificationSeverity = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationSeverity;

public sealed record NotificationResponse(
    Guid Id,
    string Category,
    string Title,
    string Body,
    NotificationSeverity Severity,
    string? ResourceType,
    Guid? ResourceId,
    string? ActionUrl,
    DateTimeOffset? ReadAt,
    DateTimeOffset CreatedAt);
