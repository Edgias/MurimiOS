using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Notification;
using GetPreferencesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Notification.GetPreferences.GetPreferencesEndpoint;
using MarkReadEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Notification.MarkRead.MarkReadEndpoint;
using PutPreferenceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Notification.PutPreference.PutPreferenceEndpoint;


public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Notification.Get.GetEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Notification.Create.CreateEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Notification.MarkRead.MarkReadEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Notification.GetPreferences.GetPreferencesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Notification.PutPreference.PutPreferenceEndpoint.Map(api);
    }
}
