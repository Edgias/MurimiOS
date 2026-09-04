using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange;
using AcknowledgeEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Acknowledge.AcknowledgeEndpoint;
using CreateBreedSubmissionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateBreedSubmission.CreateBreedSubmissionEndpoint;
using CreateMilkSubmissionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateMilkSubmission.CreateMilkSubmissionEndpoint;
using GetSubmissionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.GetSubmission.GetSubmissionEndpoint;
using InstallDefaultsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.InstallDefaults.InstallDefaultsEndpoint;
using ListDeadlinesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListDeadlines.ListDeadlinesEndpoint;
using ListProfilesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListProfiles.ListProfilesEndpoint;
using ListSubmissionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListSubmissions.ListSubmissionsEndpoint;


public static class LivestockExchangeEndpoints
{

    public static void Map(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListProfiles.ListProfilesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.InstallDefaults.InstallDefaultsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateProfile.CreateProfileEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateBreedSubmission.CreateBreedSubmissionEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.CreateMilkSubmission.CreateMilkSubmissionEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListSubmissions.ListSubmissionsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.GetSubmission.GetSubmissionEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Submit.SubmitEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Acknowledge.AcknowledgeEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListDeadlines.ListDeadlinesEndpoint.Map(api);
    }
}
