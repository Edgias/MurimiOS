using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
namespace EdgIQ.MurimiOS.Application.Notification.PutPreference;
public sealed record NotificationPreferenceResponse(string Category, bool InAppEnabled, bool EmailEnabled, bool SmsEnabled, uint Version);
