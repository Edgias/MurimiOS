using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Notification.Get;
using NotificationResponse = global::EdgIQ.MurimiOS.API.Endpoints.Notification.Shared.NotificationResponse;
public sealed record NotificationInboxResponse(int UnreadCount, IReadOnlyList<NotificationResponse> Notifications);
