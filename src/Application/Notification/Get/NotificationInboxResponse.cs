using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Notification.Get;
public sealed record NotificationInboxResponse(int UnreadCount, IReadOnlyList<NotificationResponse> Notifications);
