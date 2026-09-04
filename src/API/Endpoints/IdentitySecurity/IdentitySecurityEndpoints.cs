using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity;
using CancelRecoveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CancelRecovery.CancelRecoveryEndpoint;
using CreateRatePolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRatePolicy.CreateRatePolicyEndpoint;
using CreateRecoveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRecovery.CreateRecoveryEndpoint;
using CreateRotationPolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRotationPolicy.CreateRotationPolicyEndpoint;
using ListRatePoliciesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRatePolicies.ListRatePoliciesEndpoint;
using ListRecoveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRecovery.ListRecoveryEndpoint;
using ListRotationPoliciesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRotationPolicies.ListRotationPoliciesEndpoint;
using ListSessionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListSessions.ListSessionsEndpoint;
using RetireRatePolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RetireRatePolicy.RetireRatePolicyEndpoint;
using RetireRotationPolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RetireRotationPolicy.RetireRotationPolicyEndpoint;
using RevokeDeviceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeDevice.RevokeDeviceEndpoint;
using RevokeSessionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeSession.RevokeSessionEndpoint;
using VerifyRecoveryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.VerifyRecovery.VerifyRecoveryEndpoint;


public static class IdentitySecurityEndpoints
{
    public static void MapIdentitySecurityEndpoints(IEndpointRouteBuilder endpoints, RouteGroupBuilder api)
    {
        RouteGroupBuilder group = api.MapGroup("/security").WithTags("Identity security");
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListDevices.ListDevicesEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeDevice.RevokeDeviceEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListSessions.ListSessionsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RevokeSession.RevokeSessionEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListEvents.ListEventsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRatePolicies.ListRatePoliciesEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRatePolicy.CreateRatePolicyEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RetireRatePolicy.RetireRatePolicyEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRotationPolicies.ListRotationPoliciesEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRotationPolicy.CreateRotationPolicyEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.RetireRotationPolicy.RetireRotationPolicyEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListRecovery.ListRecoveryEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRecovery.CreateRecoveryEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CancelRecovery.CancelRecoveryEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.VerifyRecovery.VerifyRecoveryEndpoint.Map(endpoints);
    }
}
