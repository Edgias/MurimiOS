using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Notification.PutPreference;

public sealed record UpdateNotificationPreferenceRequest(bool InAppEnabled, bool EmailEnabled, bool SmsEnabled);
