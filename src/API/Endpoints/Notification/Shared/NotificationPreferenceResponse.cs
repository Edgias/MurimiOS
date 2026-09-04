using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Notification.Shared;
public sealed record NotificationPreferenceResponse(string Category, bool InAppEnabled, bool EmailEnabled, bool SmsEnabled, uint Version);
