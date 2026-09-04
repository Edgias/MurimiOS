using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.CreateRatePolicy;
public sealed record CreateApiRateLimitPolicyRequest(Guid? Id, Guid? PublicApiClientId, int RequestLimit, int WindowSeconds, int DailyQuota);
